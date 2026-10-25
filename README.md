# SignPlay — 실시간 수어 인식 교육 게임

웹캠으로 입력한 손동작을 인식해 글자 단위로 퀴즈를 풀어가는 Unity 팀 프로젝트입니다. 이 저장소는 기존 Windows 실행 배포본과 개발 원본, 기획 자료, 학습 기록을 함께 보관합니다.

| 구분 | 내용 |
| --- | --- |
| 프로젝트 | Team YOLO · 언톡 / SignPlay |
| 개발 기간 | 2025.09.13–2026.01.16 (기존 저장소명 기준) |
| 개발 환경 | Unity 2022.3.62f1 · C# · Python · MediaPipe · scikit-learn · Joblib |
| 본인 담당 | **퀴즈 씬 전체 구현 및 기존 수어 인식 모델 연동** |

## 본인 기여

팀원이 만든 인식 모델(`model.joblib`, `mnts.joblib`)을 받아 Unity 퀴즈 흐름에 연결하고, 퀴즈 씬의 기능을 구현했습니다. **모델 자체의 학습·제작은 팀원 기여**입니다. 역할 구분은 저장소 소유자의 설명을 기준으로 정리했으며, 학습용 파일이나 팀 전체 코드를 개인 단독 성과로 표기하지 않습니다.

| 구현 내용 | 확인할 코드 |
| --- | --- |
| 인식 결과를 퀴즈 정답 처리와 연결 | [UnityCameraSender.cs](source/unity/SignPlay/Assets/scripts/Quiz/UnityCameraSender.cs), [UdpReceiver.cs](source/unity/SignPlay/Assets/scripts/Quiz/UdpReceiver.cs), [Python 서버](source/python/unity_stream_server.py) |
| 맞힌 글자를 회색으로 표시하고 다음 글자·문제로 진행 | [QuizCore.cs](source/unity/SignPlay/Assets/scripts/Quiz/QuizRecognize/QuizCore.cs) |
| 퀴즈 진행 상태와 결과 관리 | [QuizGameManager.cs](source/unity/SignPlay/Assets/scripts/Quiz/QuizRecognize/QuizGameManager.cs), [QuizUI.cs](source/unity/SignPlay/Assets/scripts/Quiz/QuizRecognize/QuizUI.cs) |
| 퀴즈 이미지·배경 및 결과 이미지 전환 | [QuizBackgroundSwitcher.cs](source/unity/SignPlay/Assets/scripts/Quiz/QuizBackgroundSwitcher.cs), [QuizEndingImageSwitcher.cs](source/unity/SignPlay/Assets/scripts/Quiz/EndingScene/QuizEndingImageSwitcher.cs) |
| 힌트 및 결과 별 표시 | [HintUI.cs](source/unity/SignPlay/Assets/scripts/Quiz/Hint/HintUI.cs), [StarsAppear.cs](source/unity/SignPlay/Assets/scripts/Quiz/EndingScene/StarsAppear.cs) |

본인 담당을 자세히 보려면 [기여 및 코드 안내](docs/CONTRIBUTIONS.md)를, 전체 프로젝트 구성은 [개발 구조와 실행 안내](docs/DEVELOPMENT.md)를 참고하세요.

## 저장소 둘러보기

| 위치 | 성격 |
| --- | --- |
| [source/unity/SignPlay](source/unity/SignPlay) | 팀 게임의 Unity 개발 원본. 퀴즈 외 맵·스토리·상점 코드도 포함 |
| [source/python](source/python) | 로컬에 보관되어 있던 인식 연동 코드와 팀 제공 모델 2개 |
| [docs/project-materials](docs/project-materials) | 퀴즈 정리 PDF 및 스토리·기획 이미지 |
| [study/unity-practice](study/unity-practice) | 별도 Unity 연습 프로젝트 3개. 완성 게임과 구분 |
| [study/textbook-reference](study/textbook-reference) | 교재 예제 원본 기반 학습 스냅샷. 개인 창작물로 주장하지 않음 |
| 루트의 `SignPlay.exe`, `SignPlay_Data`, `MonoBleedingEdge` 등 | 기존 Windows 배포본. 이번 원본 정리에서 재빌드하지 않음 |

## 확인 범위

2026-10-26에 로컬 `Untoc` 폴더의 보존 대상 파일을 정리했습니다. 이 날짜는 **아카이브 정리일**이며 개발일·개별 작성 이력을 증명하는 날짜가 아닙니다. 원본의 코드·모델·에셋 바이트는 수정하지 않았습니다. 로컬 스냅샷과 기존 배포본이 같은 시점의 최종 버전이라는 보장은 없습니다.

- [보존 파일 목록 및 SHA-256](docs/ARCHIVE_MANIFEST.csv)
- [출처·팀 기여·외부 에셋 안내](docs/ATTRIBUTION.md)
- [제외 항목과 백업 범위](docs/ARCHIVE.md)

실행하려면 Git LFS를 포함해 저장소를 내려받은 뒤 Windows에서 기존 `SignPlay.exe`를 실행합니다. 개발 원본은 Unity Hub에서 `source/unity/SignPlay`를 열면 됩니다. 웹캠과 Unity/Python 실제 연동 실행은 이번 아카이브 작업에서 검증하지 않았습니다.
