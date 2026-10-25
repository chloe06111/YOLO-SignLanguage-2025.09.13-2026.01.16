using System; // Action 사용
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class UnityCameraSender : MonoBehaviour
{
    [Header("Camera")]
    public RawImage cameraRawImage;
    public int cameraIndex = 0;

    [Header("Python Server")]
    public string serverIP = "127.0.0.1";
    public int serverPort = 6000;
    public float sendInterval = 0.1f;
    [Range(10, 100)] public int jpgQuality = 50;

    private WebCamTexture webcamTexture;
    private Texture2D tex2D;

    private TcpClient client;
    private NetworkStream stream;
    private float sendTimer = 0f;

    void Start()
    {
        // 1) 웹캠 시작
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogWarning("[UnityCameraSender] No webcam found");
            return;
        }

        cameraIndex = Mathf.Clamp(cameraIndex, 0, devices.Length - 1);
        string camName = devices[cameraIndex].name;

        // 해상도나 FPS를 낮추면 끄는 속도가 빨라질 수 있음 (640x480 추천)
        webcamTexture = new WebCamTexture(camName, 640, 480);

        if (cameraRawImage != null)
        {
            cameraRawImage.texture = webcamTexture;
        }
        webcamTexture.Play();

        // 2) Python 서버(TCP) 연결
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, serverPort);
            stream = client.GetStream();
            Debug.Log("[UnityCameraSender] Connected to Python server");
        }
        catch (Exception e)
        {
            Debug.LogWarning("[UnityCameraSender] Cannot connect to Python: " + e.Message);
        }
    }

    void Update()
    {
        if (webcamTexture == null || !webcamTexture.isPlaying) return;
        if (client == null || !client.Connected) return;

        sendTimer += Time.deltaTime;
        if (sendTimer >= sendInterval && webcamTexture.didUpdateThisFrame)
        {
            sendTimer = 0f;
            SendFrameToPython();
        }
    }

    void SendFrameToPython()
    {
        try
        {
            if (tex2D == null || tex2D.width != webcamTexture.width || tex2D.height != webcamTexture.height)
            {
                tex2D = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
            }

            tex2D.SetPixels32(webcamTexture.GetPixels32());
            tex2D.Apply();

            // RGB <-> BGR 변환
            Color32[] pixels = tex2D.GetPixels32();
            for (int i = 0; i < pixels.Length; i++)
            {
                byte r = pixels[i].r;
                pixels[i].r = pixels[i].b;
                pixels[i].b = r;
            }
            tex2D.SetPixels32(pixels);
            tex2D.Apply();

            byte[] jpgBytes = tex2D.EncodeToJPG(jpgQuality);
            byte[] lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(jpgBytes.Length));

            stream.Write(lengthBytes, 0, lengthBytes.Length);
            stream.Write(jpgBytes, 0, jpgBytes.Length);
            stream.Flush();
        }
        catch (Exception)
        {
            // 전송 중 에러는 무시
        }
    }

    // ================================================================
    // ✅ 강력한 카메라 종료 함수 (콜백 지원)
    // ================================================================
    public void StopCamera(Action onComplete = null)
    {
        // 1. TCP 연결 끊기
        try
        {
            if (stream != null) { stream.Close(); stream = null; }
            if (client != null) { client.Close(); client = null; }
        }
        catch (Exception) { }

        // 2. 웹캠 끄기 및 파괴
        if (webcamTexture != null)
        {
            Debug.Log("[CameraSender] 웹캠 리소스 해제 중...");

            // 멈추고
            if (webcamTexture.isPlaying)
                webcamTexture.Stop();

            // 파괴하고 (하드웨어 연결 해제)
            Destroy(webcamTexture);
            webcamTexture = null;
        }

        // 3. 완료 신호
        onComplete?.Invoke();
    }

    // 씬이 바뀌거나 오브젝트가 파괴될 때
    void OnDestroy()
    {
        StopCamera();
    }

    // 오브젝트가 비활성화될 때 (추가 안전장치)
    void OnDisable()
    {
        StopCamera();
    }

    // ⭐ 에디터 플레이 종료 또는 게임 종료 시 (가장 중요)
    void OnApplicationQuit()
    {
        StopCamera();

        // 에디터 종료 시에는 Resources.UnloadUnusedAssets()를 
        // 동기적으로 호출하기 애매하므로 생략하거나, 
        // 필요하다면 아래 코드를 쓸 수 있지만 보통 Stop()과 Destroy()면 충분합니다.
    }
}