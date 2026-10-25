import argparse, os, time, glob, collections
import cv2, numpy as np
import mediapipe as mp
from sklearn.neighbors import KNeighborsClassifier
from sklearn.preprocessing import StandardScaler
from sklearn.pipeline import Pipeline
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report, accuracy_score, confusion_matrix
import joblib

#py unity_stream_server.py
###############################################
#  FEATURE FUNCTION (63 FEATURES FULL VERSION)
###############################################

def _angle(a, b, c):
    v1, v2 = a - b, c - b
    n1, n2 = np.linalg.norm(v1) + 1e-6, np.linalg.norm(v2) + 1e-6
    cos = np.dot(v1, v2) / (n1 * n2)
    return np.arccos(np.clip(cos, -1, 1))

def _dist(p, q):
    return float(np.linalg.norm(p - q))

def landmarks_to_feature(hand_landmarks, w, h, handed='Left', mirror_right=True):
    pts = np.array([[lm.x * w, lm.y * h] for lm in hand_landmarks.landmark], dtype=np.float32)

    # z 제거
    pts = pts[:, :2]

    # 오른손 좌우 반전
    if mirror_right and handed == 'Right':
        pts[:, 0] = w - pts[:, 0]

    # 중심정렬 + 크기정규화
    wrist = pts[0].copy()
    pts -= wrist
    size = np.linalg.norm(pts.max(0) - pts.min(0)) + 1e-6
    pts /= size

    # 기본 42개 landmark 좌표
    feat = pts.reshape(-1).tolist()

    # 손가락 index
    TH, ID, MD, RD, PD = \
        [1,2,3,4], [5,6,7,8], [9,10,11,12], [13,14,15,16], [17,18,19,20]

    def curl(idx):
        return _angle(pts[idx[0]], pts[idx[1]], pts[idx[2]]) + \
               _angle(pts[idx[1]], pts[idx[2]], pts[idx[3]])

    # curl angle (47)
    feat += [curl(TH), curl(ID), curl(MD), curl(RD), curl(PD)]

    # fingertip distances (50)
    feat += [
        _dist(pts[8], pts[12]),
        _dist(pts[12], pts[16]),
        _dist(pts[16], pts[20])
    ]

    # thumb-palm distances (59)
    thumb_tip = pts[4]
    palm_center = (pts[5] + pts[9] + pts[13] + pts[17]) / 4.0

    feat += [
        _dist(thumb_tip, pts[8]),
        _dist(thumb_tip, pts[12]),
        _dist(thumb_tip, pts[16]),
        _dist(thumb_tip, pts[20]),
        _dist(thumb_tip, pts[5]),
        _dist(thumb_tip, pts[9]),
        _dist(thumb_tip, pts[13]),
        _dist(thumb_tip, pts[17]),
        _dist(thumb_tip, palm_center)
    ]

    # index average (61)
    index_mid = (pts[5] + pts[9]) / 2.0
    index_tipm = (pts[8] + pts[12]) / 2.0
    feat += [_dist(thumb_tip, index_mid), _dist(thumb_tip, index_tipm)]

    # y-coordinate stats (63)
    tips_y = np.array([pts[4][1], pts[8][1], pts[12][1], pts[16][1], pts[20][1]])
    feat += [float(tips_y.std()), float(tips_y.mean())]

    return np.array(feat, np.float32)



###########################################################
# 아래는 수집/훈련 기능 (원래 네 파일 그대로 유지)
###########################################################

mp_hands = mp.solutions.hands
mp_draw = mp.solutions.drawing_utils


def save_samples(label, samples, out_dir):
    os.makedirs(out_dir, exist_ok=True)
    path = os.path.join(out_dir, f"{label}.csv")
    with open(path, "ab") as f:
        np.savetxt(f, samples, delimiter=",")
    print(f"[saved] {samples.shape[0]} samples -> {path}")


def load_dataset(data_dir):
    X, y = [], []
    for csv_path in glob.glob(os.path.join(data_dir, "*.csv")):
        label = os.path.splitext(os.path.basename(csv_path))[0]
        arr = np.loadtxt(csv_path, delimiter=",")
        if arr.ndim == 1:
            arr = arr[None, :]
        X.append(arr[:, 1:])
        y += [label] * arr.shape[0]
    if not X:
        raise FileNotFoundError("no data found")
    return np.vstack(X), np.array(y)


def run_collect(label, samples, out_dir, cam_id=0):
    cap = cv2.VideoCapture(cam_id, cv2.CAP_DSHOW)
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 960)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 540)

    collected = []
    print("[collect] 'c'=1샷, 'a'=자동, 'q'=종료")

    with mp_hands.Hands(max_num_hands=1,
                        min_detection_confidence=0.6,
                        min_tracking_confidence=0.6) as hands:
        auto = False
        while True:
            ret, frame = cap.read()
            if not ret:
                break

            rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            res = hands.process(rgb)

            h, w = frame.shape[:2]

            if res.multi_hand_landmarks:
                hand = res.multi_hand_landmarks[0]

                handed = "Left"
                if res.multi_handedness:
                    handed = res.multi_handedness[0].classification[0].label

                mp_draw.draw_landmarks(frame, hand, mp_hands.HAND_CONNECTIONS)

                feat = landmarks_to_feature(hand, w, h, handed)

                if auto and len(collected) < samples:
                    collected.append(np.concatenate([[hash(label) % 10], feat]))

            txt = f"Label={label} Collected={len(collected)}/{samples}"
            cv2.putText(frame, txt, (8, 24),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.7,
                        (255, 255, 255), 2)

            cv2.imshow("Collect", frame)
            k = cv2.waitKey(1) & 0xFF

            if k == ord('q'):
                break
            if k == ord('a'):
                auto = not auto
            if k == ord('c') and res.multi_hand_landmarks and len(collected) < samples:
                hand = res.multi_hand_landmarks[0]
                handed = res.multi_handedness[0].classification[0].label
                feat = landmarks_to_feature(hand, w, h, handed)
                collected.append(np.concatenate([[hash(label) % 10], feat]))

            if len(collected) >= samples:
                break

    cap.release()
    cv2.destroyAllWindows()

    if collected:
        save_samples(label, np.vstack(collected), out_dir)


def run_train(data_dir, model_path):
    X, y = load_dataset(data_dir)
    Xtr, Xte, ytr, yte = train_test_split(X, y, test_size=0.2,
                                         stratify=y, random_state=42)

    clf = Pipeline([
        ("scaler", StandardScaler()),
        ("knn", KNeighborsClassifier(n_neighbors=7, weights="distance"))
    ])

    clf.fit(Xtr, ytr)

    print(classification_report(yte, clf.predict(Xte)))
    print("ACC:", accuracy_score(yte, clf.predict(Xte)))
    print(confusion_matrix(yte, clf.predict(Xte)))

    joblib.dump(clf, model_path)
    print(f"[saved] {model_path}")


if __name__ == "__main__":
    ap = argparse.ArgumentParser(description="Sign Classifier")
    sub = ap.add_subparsers(dest="mode", required=True)

    ap_c = sub.add_parser("collect")
    ap_c.add_argument("--label", required=True)
    ap_c.add_argument("--samples", type=int, default=300)
    ap_c.add_argument("--out_dir", default="data")
    ap_c.add_argument("--cam", type=int, default=0)

    ap_t = sub.add_parser("train")
    ap_t.add_argument("--data_dir", default="data")
    ap_t.add_argument("--model", default="model.joblib")

    args = ap.parse_args()

    if args.mode == "collect":
        run_collect(args.label, args.samples, args.out_dir, args.cam)
    elif args.mode == "train":
        run_train(args.data_dir, args.model)

