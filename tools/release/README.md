# Windows 배포본 빌드

이 도구는 보관된 Unity 게임 빌드를 재사용하고, Python 인식 서버를 모델과 함께 독립 실행파일로 다시 만듭니다. Unity 게임 자체를 재컴파일하는 스크립트는 아닙니다.

Windows x64와 Python 3.10에서 저장소 루트 기준으로 실행합니다.

```powershell
python -m venv .venv
.venv\Scripts\python -m pip install -r tools/release/requirements-windows.lock.txt
.venv\Scripts\python tools/release/build_windows.py --output release-build
```

`release-build/SignPlay-Windows-x64.zip`이 한 번에 다운로드할 배포본입니다. 압축을 모두 풀고 `SignPlay.exe`를 실행합니다. 배포 대상 PC에는 Python·Unity Editor·Git 설치가 필요 없습니다.

`recognition_entry.py`는 기존 인식 코드를 수정하지 않고 감싸는 배포 진입점입니다. 모델 두 개와 MediaPipe의 초기화를 확인하는 `--self-test` 옵션 및 `%LOCALAPPDATA%/SignPlay/recognition-server.log` 로그를 제공합니다. 설치 버전 충돌을 피하기 위해 배포용 환경에는 NumPy 1.26.4와 OpenCV 배포 패키지 한 종류만 사용했습니다. JAX/TensorFlow/PyTorch는 이 Hands 인식 경로에 필요하지 않아 실행파일에서 제외했습니다.

로컬 원본 `source/python`은 그대로 보관합니다. 기존 `unity_stream_server.exe`는 Git LFS의 실제 객체가 404로 응답했으므로 이번 빌드로 교체했습니다. 기존 실행파일과 바이트가 같다는 뜻은 아닙니다.

자체 테스트와 모델 로딩만으로 실제 수어 인식 정확도 또는 모든 Windows PC에서의 호환성을 보장하지 않습니다. 배포본의 검증 범위는 [`BUILD-VALIDATION.json`](BUILD-VALIDATION.json)에 별도로 기록합니다.
