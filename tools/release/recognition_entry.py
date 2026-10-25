"""Release entry point; preserve archived Python sources unchanged."""
import json
import os
from pathlib import Path
import sys

if not getattr(sys, 'frozen', False):
    sys.path.insert(0, str(Path(__file__).resolve().parents[2] / 'source/python'))


def self_test():
    import cv2
    import joblib
    import mediapipe as mp
    import numpy as np
    from unity_stream_server import resource_path

    results = {}
    for filename in ('mnts.joblib', 'model.joblib'):
        model = joblib.load(resource_path(filename))
        features = int(model.n_features_in_)
        prediction = model.predict(np.zeros((1, features), dtype=np.float32))
        results[filename] = {'features': features, 'prediction_count': len(prediction)}
        if features != 63 or len(prediction) != 1:
            raise RuntimeError('Unexpected model feature shape')
    with mp.solutions.hands.Hands(static_image_mode=True, max_num_hands=1) as hands:
        output = hands.process(np.zeros((480, 640, 3), dtype=np.uint8))
        results['mediapipe_blank_frame'] = output.multi_hand_landmarks is None
    results['opencv'] = cv2.__version__
    results['passed'] = True
    print(json.dumps(results), flush=True)


if __name__ == '__main__':
    if '--self-test' in sys.argv:
        self_test()
    else:
        log_dir = Path(os.environ.get('LOCALAPPDATA', str(Path.home()))) / 'SignPlay'
        log_dir.mkdir(parents=True, exist_ok=True)
        log = (log_dir / 'recognition-server.log').open('w', encoding='utf-8', buffering=1)
        sys.stdout = log
        sys.stderr = log
        from unity_stream_server import main
        main()
