using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UdpGestureReceiver : MonoBehaviour
{
    [Serializable]
    private class PredDTO  // 👈 내부 클래스로 변경 (오류 원인 제거)
    {
        public string gesture;
        public float confidence;
        public double ts;
    }

    public int listenPort = 9000;
    public string currentGesture = "...";
    public float currentConfidence = 0f;

    private UdpClient udp;
    private IPEndPoint any;

    void Start()
    {
        udp = new UdpClient(listenPort);
        udp.Client.ReceiveTimeout = 1;
        any = new IPEndPoint(IPAddress.Any, 0);
        Debug.Log($"UDP Listening on port {listenPort}");
    }

    void Update()
    {
        // 수신 루프
        while (udp != null && udp.Available > 0)
        {
            try
            {
                byte[] data = udp.Receive(ref any);
                string json = Encoding.UTF8.GetString(data);
                PredDTO dto = JsonUtility.FromJson<PredDTO>(json);
                if (dto != null && !string.IsNullOrEmpty(dto.gesture))
                {
                    currentGesture = dto.gesture;
                    currentConfidence = dto.confidence;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"UDP recv error: {e.Message}");
            }
        }

        Debug.Log($"Gesture: {currentGesture} ({currentConfidence:F2})");
    }

    void OnDestroy()
    {
        try { udp?.Close(); } catch { }
        udp = null;
    }
}
