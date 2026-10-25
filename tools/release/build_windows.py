"""Build the recognition server and one-download portable ZIP on Windows."""
from pathlib import Path
import argparse
import hashlib
import json
import shutil
import subprocess
import sys
import zipfile

ROOT = Path(__file__).resolve().parents[2]
TOOLS = Path(__file__).resolve().parent


def build(output, package_only=False):
    output = output.resolve()
    output.mkdir(parents=True, exist_ok=True)
    python_source = ROOT / 'source/python'
    cmd = [sys.executable, '-m', 'PyInstaller', '--noconfirm', '--clean', '--onefile',
           '--console', '--name', 'unity_stream_server',
           '--distpath', str(output/'server'), '--workpath', str(output/'work'),
           '--specpath', str(output/'spec'), '--paths', str(python_source),
           '--collect-all', 'mediapipe',
           '--add-data', str(python_source/'model.joblib')+';.',
           '--add-data', str(python_source/'mnts.joblib')+';.' ]
    for module in ['jax', 'jaxlib', 'tensorflow', 'torch', 'IPython', 'pytest', 'tkinter']:
        cmd += ['--exclude-module', module]
    cmd.append(str(TOOLS/'recognition_entry.py'))
    if not package_only:
        subprocess.run(cmd, check=True)
    server = output/'server/unity_stream_server.exe'
    subprocess.run([str(server), '--self-test'], check=True, timeout=120, creationflags=subprocess.CREATE_NO_WINDOW)
    package = output/'SignPlay-Windows-x64'
    package.mkdir(exist_ok=True)
    for name in ['SignPlay.exe', 'UnityPlayer.dll', 'UnityCrashHandler64.exe', 'LICENSE']:
        shutil.copy2(ROOT/name, package/name)
    for name in ['SignPlay_Data', 'MonoBleedingEdge']:
        shutil.copytree(ROOT/name, package/name, dirs_exist_ok=True)
    shutil.copy2(server, package/'unity_stream_server.exe')
    (package/'README_KO.txt').write_text('''SignPlay — Windows x64 체험판

1. ZIP 전체를 다운로드한 뒤 압축을 모두 풀어주세요.
2. 압축을 푼 폴더에서 SignPlay.exe를 실행하세요.
3. 인식 서버가 준비될 때까지 기다린 뒤 퀴즈에서 웹캠 사용을 허용하세요.

Python, Unity Editor, Git 설치는 필요 없습니다.
SignPlay_Data, MonoBleedingEdge, DLL, unity_stream_server.exe는 함께 두세요.
ZIP 안에서 바로 실행하거나 SignPlay.exe만 다른 위치로 옮기면 작동하지 않습니다.

수어 인식에는 웹캠이 필요합니다. Windows 설정 > 개인정보 및 보안 > 카메라에서
데스크톱 앱의 카메라 접근을 허용하고 다른 앱의 카메라 사용을 종료하세요.
인식 서버 로그: %LOCALAPPDATA%\\SignPlay\\recognition-server.log
게임 로그: %USERPROFILE%\\AppData\\LocalLow 아래 게임 Player.log

Team YOLO 팀 프로젝트입니다. 저장소 소유자는 퀴즈 씬 전체 구현과
팀 제공 수어 인식 모델의 연동을 담당했습니다. 모델 제작은 팀원 기여입니다.
게임은 기존 Unity 배포본이며, 인식 서버는 보관된 코드와 모델로 다시 빌드했습니다.
별도 PC에서의 전체 플레이와 실제 손동작 인식 정확도는 아직 검증하지 않았습니다.

소스 및 설명:
https://github.com/chloe06111/YOLO-SignLanguage-2025.09.13-2026.01.16
''', encoding='utf-8-sig')
    hashes = []
    for p in sorted(package.rglob('*')):
        if p.is_file():
            hashes.append(hashlib.sha256(p.read_bytes()).hexdigest()+'  '+p.relative_to(package).as_posix())
    (package/'SHA256SUMS.txt').write_text('\n'.join(hashes)+'\n',encoding='utf-8')
    archive = output/'SignPlay-Windows-x64.zip'
    with zipfile.ZipFile(archive,'w',compression=zipfile.ZIP_DEFLATED,compresslevel=6) as z:
        for p in sorted(package.rglob('*')):
            if p.is_file(): z.write(p,p.relative_to(package.parent))
    digest=hashlib.sha256(archive.read_bytes()).hexdigest()
    (output/'SHA256SUMS.txt').write_text(digest+'  '+archive.name+'\n',encoding='utf-8')
    report={'zip':str(archive),'bytes':archive.stat().st_size,'sha256':digest,'server_bytes':server.stat().st_size}
    (output/'build-report.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
    print(json.dumps(report))


if __name__ == '__main__':
    parser=argparse.ArgumentParser()
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--package-only', action='store_true', help='Reuse the existing frozen server; still run self-test')
    args=parser.parse_args()
    build(args.output, args.package_only)
