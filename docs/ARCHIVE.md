# 보존 범위

정리일: 2026-10-26 (Asia/Seoul).

로컬 Untoc 폴더에서 Unity 프로젝트의 Assets·Packages·ProjectSettings, Python 코드와 모델 2개, PDF와 이미지 자료를 보존했습니다. 파일명 매핑과 SHA-256/Git blob 해시는 ARCHIVE_MANIFEST.csv에 있습니다. 원본은 재작성하지 않았습니다.

제외: Unity Library·Temp·Obj·Logs·UserSettings, Python .venv·__pycache__, IDE 설정, .git·.plastic 내부 메타데이터, __MACOSX, desktop.ini, 자동 생성 .csproj/.sln 및 UpgradeLog. 이들 중 Library만 로컬 공간 정리 대상으로 삼으며 원본·모델·PDF·이미지는 삭제하지 않습니다.

기존 루트 배포본 및 기존 Git 이력은 유지합니다. 소스 스냅샷이 기존 exe와 정확히 같은 빌드라는 보장은 없습니다. Python 학습 CSV와 별도의 최종 Unity 클라우드 버전은 발견/검증하지 못했습니다. 모델은 원본 그대로 보존했으며 정확도와 재학습 가능성을 검증하지 않았습니다.

인증키 형식과 비밀정보 파일명에 대한 자동 점검을 실시했지만 모든 개인정보나 권리 관계를 보증하는 검사는 아닙니다. 경로는 로컬 사용자 절대경로 대신 Untoc 기준 상대경로로 기록합니다.
