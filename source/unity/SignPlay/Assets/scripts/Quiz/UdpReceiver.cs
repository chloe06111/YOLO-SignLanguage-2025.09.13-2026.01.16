//카메라 출력 부분:
using TMPro;

using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

// 5052 포트에서 UDP 데이터를 수신하고 콘솔에 출력하는 스크립트
public class UdpReceiver : MonoBehaviour
{
    // ⭐️ Python의 Send_test.py 포트(5052)와 일치시킵니다.
    public int port = 5052;

    private UdpClient client;
    private IPEndPoint anyIP;

//밑에 코드 중요 -> 이게 퀴즈 인식하는 데이터 값 어디에서 찾는지? 도와줌 
    public QuizCore quizCore;

//Pred 표시용 TMP
    public TextMeshProUGUI predText;

    void Start()
    {
        try
        {
            client = new UdpClient(port);
            anyIP = new IPEndPoint(IPAddress.Any, 0); // 모든 IP로부터 데이터를 받음
            client.Client.Blocking = false; // ⭐️ 비동기(non-blocking) 모드

            // ⭐️⭐️ 수정된 부분 ⭐️⭐️ (오류가 나지 않도록 한 줄로 합쳤습니다)
            Debug.Log($"UDP Receiver started on port {port}. Waiting for data...");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start UDP client on port {port}: {e.Message}");
        }
    }

    void Update()
    {
        if (client == null) return;

        try
        {
            byte[] data = client.Receive(ref anyIP);

            if (data.Length > 0)
            {
                string message = Encoding.UTF8.GetString(data);

                // ⭐️ Unity 콘솔에 수신된 메시지를 출력합니다!
                Debug.Log($"Received from {anyIP.Address}: {message}");

                // ⭐️⭐️ 추가: 앞뒤 공백 제거 + 소문자로 변환 ⭐️⭐️
                string cleaned = message.Trim().ToLower(); 

                if (predText != null && !string.IsNullOrEmpty(cleaned))
                {
                    predText.text = "alphabet: " + cleaned.ToUpper();
                }

                //추가: QuizCore 연결 확인 -> 인식하는데 있어서 제일 데이터 받아서 사용하게끔 만드는 코드임
                if (quizCore != null)
                {
                    quizCore.CheckAnswer(cleaned);
                    Debug.Log($"[UdpReceiver] Sent to QuizCore: {cleaned}");
                }
                else
                {
                    Debug.LogWarning("[UdpReceiver] quizCore reference is NULL!");
                }
            }
        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode != SocketError.WouldBlock)
            {
                Debug.LogWarning($"UDP Receive error: {e.Message}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in UDP Update: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        if (client != null)
        {
            client.Close();
            Debug.Log("UDP Receiver closed.");
        }
    }
}
