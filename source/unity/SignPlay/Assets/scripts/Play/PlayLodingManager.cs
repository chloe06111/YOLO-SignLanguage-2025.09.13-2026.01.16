using UnityEngine;
using TMPro; // 🔥 [수정] 이게 있어야 TextMeshPro를 쓸 수 있습니다!
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("Settings")]
    public string nextSceneName = "MainMapScene";
    public float minLoadingTime = 3.0f;

    [Header("UI Settings")]
    // 🔥 [수정] Text -> TextMeshProUGUI 로 변경
    public TextMeshProUGUI statusText;
    public float textChangeInterval = 5.0f;

    [Header("Random Messages")]
    [TextArea]
    public string[] loadingTips = new string[]
    {
        "AI가 손을 풀고 있습니다...",
        "수어 데이터를 불러오는 중입니다...",
        "카메라 연결을 확인하고 있습니다...",
        "조금만 기다려주세요!",
        "손이 잘 보이게 조명을 밝게 해주세요."
    };

    private float timer = 0f;
    private bool isMapLoadStarted = false;

    void Start()
    {
        if (statusText != null && loadingTips.Length > 0)
        {
            StartCoroutine(CycleLoadingText());
        }
    }

    void Update()
    {
        if (isMapLoadStarted || PlaySceneController.Instance == null) return;

        timer += Time.deltaTime;

        bool serverReady = false;
        if (ProcessStarter.Instance != null)
        {
            serverReady = ProcessStarter.Instance.IsServerReady;
        }
        else
        {
            serverReady = true;
        }

        if (timer >= minLoadingTime && serverReady)
        {
            isMapLoadStarted = true;
            LoadNextScene();
        }
    }

    private IEnumerator CycleLoadingText()
    {
        while (!isMapLoadStarted)
        {
            int randomIndex = Random.Range(0, loadingTips.Length);
            statusText.text = loadingTips[randomIndex];
            yield return new WaitForSecondsRealtime(textChangeInterval);
        }
    }

    private void LoadNextScene()
    {
        if (statusText != null) statusText.text = "Ready to go! Starting...";
        Debug.Log("[LoadingManager] 맵으로 이동합니다.");

        StartCoroutine(PlaySceneController.Instance.PlayLoadWithDelay(nextSceneName, 1.0f));
    }
}