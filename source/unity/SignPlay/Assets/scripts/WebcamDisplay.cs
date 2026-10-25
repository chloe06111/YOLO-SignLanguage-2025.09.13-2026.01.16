// WebcamDisplay.cs
using UnityEngine;
using UnityEngine.UI; // RawImage 사용

[RequireComponent(typeof(RawImage))]
public class WebcamDisplay : MonoBehaviour
{
    public int requestedWidth = 640;
    public int requestedHeight = 480;

    private RawImage rawImage;
    private WebCamTexture webcamTexture;

    void Start()
    {
        rawImage = GetComponent<RawImage>();

        // 1. 사용 가능한 웹캠 목록 가져오기
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("웹캠을 찾을 수 없습니다!");
            return;
        }

        // 2. 첫 번째 웹캠을 텍스처로 사용
        webcamTexture = new WebCamTexture(devices[0].name, requestedWidth, requestedHeight);

        // 3. RawImage에 웹캠 텍스처 연결
        rawImage.texture = webcamTexture;
        rawImage.material.mainTexture = webcamTexture; // (선택) 좌우 반전 셰이더 적용 시 필요

        // 4. 웹캠 재생 시작
        webcamTexture.Play();
    }

    void OnDisable()
    {
        // 씬이 꺼질 때 웹캠도 같이 종료 (중요)
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}