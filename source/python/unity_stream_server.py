import socket
import struct
import collections
import cv2
import numpy as np
import mediapipe as mp
import joblib
import sys
import os

# sign_classifier.py가 같은 폴더에 있어야 합니다.
from sign_classifier import landmarks_to_feature

TCP_PORT = 6000
UDP_IP = "127.0.0.1"
UDP_PORT = 5052
SMOOTH_WINDOW = 5
CONF_THRESH = 0.3

mp_hands = mp.solutions.hands
mp_draw = mp.solutions.drawing_utils


# -----------------------------------------------------------
# [1] 리소스 경로 함수 (PyInstaller 호환용)
# -----------------------------------------------------------
def resource_path(relative_path):
    try:
        base_path = sys._MEIPASS
    except Exception:
        base_path = os.path.abspath(".")
    return os.path.join(base_path, relative_path)


def recv_exact(conn, n: int):
    buf = b""
    while len(buf) < n:
        chunk = conn.recv(n - len(buf))
        if not chunk:
            return None
        buf += chunk
    return buf


def main():
    print("[Python] 모델 파일 로딩 시작...")
    try:
        scaler = joblib.load(resource_path('mnts.joblib'))
        print("[Python] Scaler (mnts.joblib) 로드 완료")

        clf = joblib.load(resource_path('model.joblib'))
        print("[Python] Classifier (model.joblib) 로드 완료")

    except Exception as e:
        print(f"[Python] 모델 로드 실패: {e}")
        return

    udp_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    server_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server_sock.bind(("0.0.0.0", TCP_PORT))
    server_sock.listen(1)

    while True:
        print(f"\n[Python] Unity 연결 대기중... (TCP {TCP_PORT})")
        conn, addr = server_sock.accept()
        print(f"[Python] Unity 접속됨: {addr}")

        q = collections.deque(maxlen=SMOOTH_WINDOW)

        with mp_hands.Hands(
            max_num_hands=1,
            min_detection_confidence=0.7,
            min_tracking_confidence=0.7
        ) as hands:

            while True:
                try:
                    length_bytes = recv_exact(conn, 4)
                    if not length_bytes:
                        print("[Python] Unity 연결 종료 (데이터 없음)")
                        break

                    (length,) = struct.unpack("!i", length_bytes)

                    jpg_bytes = recv_exact(conn, length)
                    if not jpg_bytes:
                        break

                    np_data = np.frombuffer(jpg_bytes, np.uint8)
                    frame = cv2.imdecode(np_data, cv2.IMREAD_COLOR)
                    if frame is None:
                        continue

                    rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
                    res = hands.process(rgb)
                    h, w = frame.shape[:2]

                    pred_txt = "None"

                    if res.multi_hand_landmarks:
                        hand = res.multi_hand_landmarks[0]

                        # (1) 특징 추출
                        feat = landmarks_to_feature(hand, w, h)
                        feat = feat.reshape(1, -1)

                        # (2) 스케일러 적용
                        try:
                            feat = scaler.transform(feat)
                        except:
                            pass  # ❗ 여기까지만 try-except

                        # ✅ (3) 모델 예측 — 들여쓰기 수정됨 (항상 실행됨)
                        if hasattr(clf, "predict_proba"):
                            proba = clf.predict_proba(feat)[0]
                            best = proba.max()
                            pred = clf.classes_[proba.argmax()]
                            pred_txt = pred if best >= CONF_THRESH else "UNKNOWN"
                        else:
                            pred_txt = clf.predict(feat)[0]

                        # (4) 스무딩
                        q.append(pred_txt)
                        pred_txt = collections.Counter(q).most_common(1)[0][0]

                    if pred_txt not in ["None", "UNKNOWN", ""]:
                        udp_sock.sendto(
                            pred_txt.encode("utf-8"),
                            (UDP_IP, UDP_PORT)
                        )

                except Exception as e:
                    print(f"[Python] 에러 발생: {e}")
                    break

        conn.close()
        print("[Python] Unity 연결 종료. 다시 접속 대기...")

    server_sock.close()
    udp_sock.close()


if __name__ == "__main__":
    main()
