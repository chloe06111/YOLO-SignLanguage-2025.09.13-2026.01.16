# YOLO



================================================================

SignPlay: 수어 교육용 기능성 게임 (Sign Language Education Game)

================================================================



"수어를 게임처럼 쉽고 재미있게!"

Unity와 Google MediaPipe를 활용하여 사용자의 손동작을 실시간으로 인식하는 

인터랙티브 수어 학습 게임입니다.





\[1. 프로젝트 소개]

SignPlay는 수어에 대한 진입 장벽을 낮추기 위해 기획된 기능성 게임 프로젝트입니다. 

웹캠을 통해 내 손을 화면 속에서 직접 보며, 퀴즈를 풀고 스토리를 진행하는 과정에서 

자연스럽게 지문자를 익힐 수 있습니다.



\- 개발 팀: Team YOLO (부산대학교 정보컴퓨터공학부 \& 전기전자공학부)

\- 주요 기술: Unity, Python, MediaPipe, Socket Communication





\[2. 주요 기능]

1\) 실시간 수어 인식

&nbsp;  - 웹캠으로 입력된 손의 관절 포인트(21개)를 MediaPipe로 추출하여 알파벳을 실시간 판별합니다.



2\) 이중 통신 구조

&nbsp;  - Unity(게임 클라이언트)와 Python(AI 서버) 간의 TCP/UDP 소켓 통신을 통해 안정적인 인식 환경을 구축했습니다.



3\) 스토리 \& 맵 시스템

&nbsp;  - '숲'과 '바다' 테마로 구성된 맵을 탐험하며 단계별로 퀴즈를 해결합니다.



4\) 상점 \& 커스터마이징

&nbsp;  - 퀴즈 보상(코인)으로 캐릭터 스킨과 UI 테마를 구매하여 게임을 꾸밀 수 있습니다.



5\) 편의성 (No Python Required)

&nbsp;  - PyInstaller를 통해 Python 환경이 없는 사용자도 별도 설치 없이 실행 가능하도록 패키징했습니다.





\[3. 기술 스택]

\- Game Engine: Unity (C#)

\- AI \& Vision: Python, Google MediaPipe, Scikit-learn (Joblib)

\- Network: Socket Programming (TCP/UDP)

\- DevOps: Unity DevOps, Unity Cloud

\- Design: Aseprite (Pixel Art Assets)





\[4. 시스템 아키텍처]

저희 프로젝트는 Unity와 Python이 동시에 실행되며 데이터를 주고받는 구조입니다.



(1) Unity (Client): 

&nbsp;   웹캠 영상을 캡처하여 TCP 통신으로 Python 서버에 전송합니다.

(2) Python (Server): 

&nbsp;   전송받은 이미지에서 MediaPipe로 손 좌표를 추출하고, 학습된 모델로 알파벳을 분류합니다.

(3) Unity (Client): 

&nbsp;   분류된 결과값을 UDP 통신으로 받아 퀴즈 정답 여부를 판단합니다.



\* 초기에는 YOLO 객체 인식을 고려했으나, 손가락의 미세한 관절 움직임을 

&nbsp; 정확히 파악하기 위해 MediaPipe와 좌표 기반 분류기로 전환하였습니다.





\[5. 설치 및 실행 방법]

1\. 배포된 압축 파일을 다운로드하여 해제합니다.

2\. 'SignPlay.exe'를 실행합니다.

3\. \[주의] 게임 시작 시 AI 서버(Python)가 로딩되는 시간이 소요될 수 있습니다. 

&nbsp;  로딩 화면이 끝날 때까지 잠시 기다려주세요.

4\. 웹캠 사용 권한을 허용하면 플레이가 시작됩니다.





\[6. 팀원 및 역할 (Team YOLO)]

\- Design Team: UI/UX 디자인, 도트 에셋 및 캐릭터 제작

\- Quiz Team: MediaPipe 손 인식 구현, 머신러닝 모델 학습, 소켓 통신 구현

\- Game Team: 유니티 게임 로직, 맵/상점/스토리 시스템 구현, 협업 환경 구축





\[7. 회고 및 향후 계획]

\- MediaPipe 도입: 

&nbsp; 손가락의 디테일한 모양 변화를 감지하기 위해 Bounding Box 방식(YOLO) 대신 

&nbsp; 관절 포인트 추적 방식(MediaPipe)을 채택하여 인식률을 높였습니다.



\- 동적 인식의 과제:  

&nbsp; 현재 버전은 정적인 지문자 인식에 최적화되어 있습니다. 추후 시계열 데이터 학습을 통해 

&nbsp; 동적인 수어 동작까지 인식할 수 있도록 발전시킬 계획입니다.





================================================================

Copyright 2026. Team YOLO. All rights reserved.

================================================================

