# 기여 및 코드 안내

## 본인 담당: 퀴즈 씬 전체

저장소 소유자는 퀴즈 씬 내부의 기능 전체를 맡았다고 설명했습니다. 구체적으로 팀원이 만들어 둔 Joblib 인식 모델을 연결하고, 인식 성공 시 글자를 회색으로 바꾸는 처리, 이미지 전환, 결과 별 표시 등을 구현했습니다.

퀴즈 코드는 [`Assets/scripts/Quiz`](../source/unity/SignPlay/Assets/scripts/Quiz)에 있습니다. Unity 씬과 Inspector 설정은 [`Assets/Scenes`](../source/unity/SignPlay/Assets/Scenes)에 함께 보관되어 있습니다.

1. `UnityCameraSender.cs`: 웹캠 프레임을 인식 서버에 전송하는 연결 지점.
2. `UdpReceiver.cs`: Python 인식 결과를 수신해 퀴즈 쪽에 전달하는 연결 지점.
3. `QuizRecognize/QuizCore.cs`: 현재 글자와 수신 글자를 비교하고, 맞힌 글자를 `<color=#808080>`로 표시한 뒤 다음 문제로 진행.
4. `QuizRecognize/QuizGameManager.cs`: 문제·진행·결과 상태 관리.
5. `QuizBackgroundSwitcher.cs`, `EndingScene/QuizEndingImageSwitcher.cs`: 퀴즈 및 결과 이미지 전환.
6. `Hint/HintUI.cs`, `Hint/HintImage.cs`: 힌트 표시 관련 기능.
7. `EndingScene/StarsAppear.cs`: 확정된 힌트 사용 횟수에 따라 별 표시. 코드 기준 0–1회는 3개, 2회는 2개, 3회 이상은 1개.

## 팀원 및 공동 산출물

- `model.joblib`, `mnts.joblib`: 팀원에게 제공받아 연결한 인식 모델 파일. 본인의 모델 학습 성과로 표기하지 않습니다.
- 맵·상점·스토리·아트 등 퀴즈 외 전체 게임의 개인별 담당은 여기서 확정하지 않습니다.
- Python 폴더의 `sign_classifier.py`에는 데이터 수집·학습 코드가 있지만, 파일 보유 사실만으로 그 전체를 개인 작성물로 귀속하지 않습니다.
- `study/textbook-reference`는 외부 교재 예제 기반입니다. `study/unity-practice`도 학습 기록으로 구분하며 모든 코드의 독자 창작을 주장하지 않습니다.

이번 정리는 파일을 새로 가져온 아카이브 커밋입니다. 예전 Git/Unity Version Control의 개인별 커밋 이력을 복원한 것은 아닙니다.
