using UnityEngine;
using UnityEngine.UI;

public class IntroSlideshow : MonoBehaviour
{
    [Header("UI")]
    public GameObject rootPanel;
    public Image slideImage;

    [Header("Slides")]
    public Sprite[] slides;

    [Header("Option")]
    public bool showOnlyOnce = true;
    public string onceKey = "IntroShown";

    [Header("Auto Play")]
    public bool autoPlayOnStart = false;

    // (선택) 스토리 재생 중 비활성화할 오브젝트
    [Header("Disable While Showing")]
    public GameObject[] disableWhileShowing;

    private int index = 0;
    private bool isShowing = false;

    // ✅ 외부(클릭 스크립트)에서 "지금 재생중인지" 확인용
    public bool IsShowing => isShowing;

    private void Start()
    {
        if (autoPlayOnStart)
        {
            // showOnlyOnce 옵션이면 이미 본 경우 자동재생 안 함
            if (showOnlyOnce && PlayerPrefs.GetInt(onceKey, 0) == 1)
                return;

            Begin();
        }
    }

    public void Begin()
    {
        if (rootPanel == null || slideImage == null)
        {
            Debug.LogWarning("[IntroSlideshow] rootPanel 또는 slideImage가 연결되지 않았습니다.");
            return;
        }

        if (slides == null || slides.Length == 0)
        {
            Debug.LogWarning("[IntroSlideshow] slides가 비어있습니다.");
            return;
        }

        // showOnlyOnce 옵션이면 이미 본 경우 실행 안 함
        if (showOnlyOnce && PlayerPrefs.GetInt(onceKey, 0) == 1)
        {
            return;
        }

        index = 0;
        isShowing = true;

        // 패널 켜기 + 첫 장 표시 (중요!)
        rootPanel.SetActive(true);
        slideImage.sprite = slides[index];

        // 재생 중 다른 UI 비활성화(선택)
        SetDisableWhileShowing(true);
    }

    public void NextFromButton()
    {
        if (!isShowing) return;

        index++;

        if (slides == null || index >= slides.Length)
        {
            Hide();
            return;
        }

        if (slideImage != null)
            slideImage.sprite = slides[index];
    }

    public void Hide()
    {
        isShowing = false;

        if (rootPanel != null)
            rootPanel.SetActive(false);

        // showOnlyOnce면 본 기록 저장
        if (showOnlyOnce)
        {
            PlayerPrefs.SetInt(onceKey, 1);
            PlayerPrefs.Save();
        }

        // 재생 끝나면 다시 활성화
        SetDisableWhileShowing(false);
    }

    private void SetDisableWhileShowing(bool disable)
    {
        if (disableWhileShowing == null) return;

        for (int i = 0; i < disableWhileShowing.Length; i++)
        {
            if (disableWhileShowing[i] != null)
                disableWhileShowing[i].SetActive(!disable);
        }
    }
}
