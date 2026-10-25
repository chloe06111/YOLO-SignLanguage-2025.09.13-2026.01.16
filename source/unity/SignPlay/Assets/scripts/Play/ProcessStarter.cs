using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Collections;

public class ProcessStarter : MonoBehaviour
{
    public static ProcessStarter Instance;

    private const string PythonExeName = "unity_stream_server.exe";

    public bool IsServerReady { get; private set; } = false;

    private Process pythonProcess;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // 🔥 [수정됨] 에디터(Unity)일 때는 무조건 통과! (서버 체크 안 함)
        if (Application.isEditor)
        {
            UnityEngine.Debug.LogWarning("💻 [에디터 모드] 서버 연결 확인을 건너뜁니다. (IsServerReady = true)");
            IsServerReady = true; // 강제 합격 처리
            return; // 더 이상 아무것도 안 하고 함수 종료
        }

        // --- 아래부터는 빌드(.exe) 환경에서만 실행됩니다 ---

        KillAllPythonProcesses();

        string exePath;
        string gameFolder;

        gameFolder = Directory.GetParent(Application.dataPath).FullName;
        exePath = Path.Combine(gameFolder, PythonExeName);

        if (!File.Exists(exePath))
        {
            UnityEngine.Debug.LogError($"[ProcessStarter] 배포 파일 없음: {exePath}");
            return;
        }

        pythonProcess = new Process();
        pythonProcess.StartInfo.FileName = exePath;
        pythonProcess.StartInfo.WorkingDirectory = gameFolder;
        pythonProcess.StartInfo.UseShellExecute = false;
        pythonProcess.StartInfo.CreateNoWindow = true;

        try
        {
            pythonProcess.Start();
            UnityEngine.Debug.Log($"[ProcessStarter] 서버 시작 (PID: {pythonProcess.Id})");
            StartCoroutine(CheckServerReadyRoutine());
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"[ProcessStarter] 실행 실패: {e.Message}");
        }
    }

    private IEnumerator CheckServerReadyRoutine()
    {
        IsServerReady = false;
        int port = 6000;
        string ip = "127.0.0.1";

        UnityEngine.Debug.Log("[ProcessStarter] 서버 접속 대기 중...");

        while (!IsServerReady)
        {
            try
            {
                TcpClient client = new TcpClient();
                var result = client.BeginConnect(ip, port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(System.TimeSpan.FromMilliseconds(500));

                if (success)
                {
                    client.EndConnect(result);
                    client.Close();
                    IsServerReady = true;
                    UnityEngine.Debug.Log("✅ [ProcessStarter] Python 서버 준비 완료!");
                }
                else
                {
                    client.Close();
                }
            }
            catch { }

            if (!IsServerReady)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    void OnApplicationQuit()
    {
        KillAllPythonProcesses();
    }

    public void KillCurrentProcess()
    {
        KillAllPythonProcesses();
    }

    private void KillAllPythonProcesses()
    {
        try
        {
            // 빌드된 게임이 아닐 땐 굳이 taskkill을 하지 않도록 예외처리를 할 수도 있습니다.
            // 하지만 안전을 위해 놔두셔도 무방합니다.
            if (!Application.isEditor)
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "taskkill";
                psi.Arguments = $"/IM {PythonExeName} /F";
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                Process killer = Process.Start(psi);
                if (killer != null) killer.WaitForExit();
            }
        }
        catch (System.Exception) { }
    }
}