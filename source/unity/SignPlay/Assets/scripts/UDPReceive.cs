// 1. Unity 프로젝트의 Assets 폴더에 'UDPReceive.cs' 파일로 저장
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
    // Python과 동일한 포트 번호 (통신 창구 번호)
    private int port = 5052; 
    
    // 수신된 문자열 데이터를 저장할 변수 (다른 스크립트에서 접근할 데이터)
    public string data = ""; 

    private Thread receiveThread;
    private UdpClient client;

    public void Start()
    {
        // 비동기 통신을 위해 새 스레드 시작
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true; 
        receiveThread.Start();
        Debug.Log("[UDP] Receiver 시작. 포트: " + port);
    }

    private void ReceiveData()
    {
        // UDP 클라이언트 생성 및 포트 초기화
        client = new UdpClient(port);
        
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                
                // 데이터 수신 (바이트 배열)
                byte[] dataBuffer = client.Receive(ref anyIP);
                
                // 바이트를 문자열로 변환 (파싱 준비)
                string receivedData = Encoding.UTF8.GetString(dataBuffer);
                
                // 데이터를 'public string data' 변수에 저장
                data = receivedData;
            }
            catch (Exception err)
            {
                // Unity 종료 시 스레드 종료 등 오류 처리
                Debug.LogWarning("[UDP] 통신 오류: " + err.ToString());
            }
        }
    }

    // Unity 앱이 종료될 때 스레드와 클라이언트 정리
    private void OnApplicationQuit()
    {
        if (receiveThread != null)
        {
            receiveThread.Abort(); 
            client.Close();
        }
    }
}