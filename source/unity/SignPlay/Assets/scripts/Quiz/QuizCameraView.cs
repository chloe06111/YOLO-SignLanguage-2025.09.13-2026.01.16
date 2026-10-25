using UnityEngine;
using UnityEngine.UI;   // RawImage 쓰려고 필요

public class QuizCameraView : MonoBehaviour
{
    public RawImage cameraRawImage;   // 카메라 화면을 보여줄 RawImage
    private WebCamTexture webcamTexture;

    void Start()
    {
        // cameraRawImage를 인스펙터에 안 넣었으면,
        // 이 스크립트가 붙은 오브젝트에서 자동으로 찾아보기
        if (cameraRawImage == null)
        {
            cameraRawImage = GetComponent<RawImage>();
        }

        // 연결된 카메라 목록 가져오기
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogWarning("[QuizCameraView] No webcam found!");
            return;
        }

        // 첫 번째 카메라 사용
        string camName = devices[0].name;
        Debug.Log("[QuizCameraView] Using camera: " + camName);

        webcamTexture = new WebCamTexture(camName);

        if (cameraRawImage != null)
        {
            cameraRawImage.texture = webcamTexture;
        }

        webcamTexture.Play();
    }

    void OnDestroy()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
