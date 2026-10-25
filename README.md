# SignPlay — 실시간 수어 인식 교육 게임

## 게임 직접 실행하기 (Windows x64)

**[▶ Windows 체험판 ZIP 한 번에 다운로드](https://github.com/chloe06111/YOLO-SignLanguage-2025.09.13-2026.01.16/releases/download/v2026.10.26/SignPlay-Windows-x64.zip)** · [배포 페이지와 검증 결과](https://github.com/chloe06111/YOLO-SignLanguage-2025.09.13-2026.01.16/releases/tag/v2026.10.26)

1. 위 ZIP을 다운로드하고 **압축을 모두 풉니다**.
2. 압축을 푼 폴더에서 **`SignPlay.exe`**를 실행합니다.
3. 인식 서버가 준비될 때까지 기다린 뒤 웹캠을 연결해 퀴즈를 체험합니다.

Python·Unity Editor·Git을 따로 설치할 필요가 없습니다. 게임과 인식 서버·모델·필요 라이브러리가 포함돼 있습니다. `SignPlay.exe`만 따로 옮기지 마세요. **GitHub의 `Code → Download ZIP` 대신 위 체험판 ZIP을 사용하세요.** 웹브라우저에서 바로 실행되는 게임은 아닙니다.

모델 2개 예측, MediaPipe 초기화, TCP 이미지 수신·재연결, Unity 시작 종료 및 ZIP 무결성을 확인했습니다. 별도 PC의 전체 플레이 및 실제 손동작 인식은 아직 검증하지 않았습니다. [상세 검증 범위](tools/release/BUILD-VALIDATION.json)


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

체험은 위 Windows 체험판 ZIP을 이용하세요. 개발 원본은 Unity Hub에서 `source/unity/SignPlay`를 열면 됩니다. 루트의 인식 서버 실행파일은 누락된 Git LFS 객체를 대체하기 위해 보관된 코드와 모델로 다시 빌드했습니다. 직접 저장소를 복제하는 개발자는 Git LFS가 필요합니다. [배포본 빌드 방법](tools/release/README.md)
