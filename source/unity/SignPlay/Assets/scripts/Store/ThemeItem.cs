using UnityEngine;
using UnityEngine.UI;

public class ThemeItem : MonoBehaviour
{
    [Header("--- Unique Settings ---")]
    public string themeID;

    [Header("--- UI Sign Settings ---")]
    public Image signImage;
    public Sprite applySprite;
    public Sprite pressedApplySprite;

    [Header("--- Theme Background Settings ---")]
    public Image themeBackgroundImage;
    public Sprite themeSprite;

    [Header("--- Transform Settings ---")]
    public Vector2 themePosition;
    public Vector2 themeSize;

    [Header("--- Selection Settings ---")]
    public GameObject selectionFrame;
    public RectTransform selectionTarget;

    [Header("--- Cost Settings ---")]
    public int themeCost = 500;

    [Header("--- Setup ---")]
    public int initialState = 0; // 기본 배경은 1로 설정
    private int state;

    void OnEnable()
    {
        string selectedTheme = PlayerPrefs.GetString("SelectedTheme", "default");

        if (themeID == selectedTheme)
        {
            state = 2;
        }
        else if (PlayerPrefs.GetInt(themeID + "_Purchased", 0) == 1 || (initialState == 1 && themeID == "default"))
        {
            state = 1;
        }
        else
        {
            state = initialState;
        }

        RefreshUI();

        if (state == 2)
        {
            MoveSelectionFrame();
            UpdatePreviewBackground(); // 탭을 넘나들 때도 배경 유지
        }
    }

    void RefreshUI()
    {
        if (signImage != null)
        {
            if (state == 1 && applySprite != null)
            {
                signImage.sprite = applySprite;
                signImage.SetNativeSize();
            }
            else if (state == 2 && pressedApplySprite != null)
            {
                signImage.sprite = pressedApplySprite;
                signImage.SetNativeSize();
            }
        }
    }

    public void OnClickItem()
    {
        if (state == 0) // 구매
        {
            if (QuizGameManager.Instance != null && QuizGameManager.Instance.coin >= themeCost)
            {
                QuizGameManager.Instance.coin -= themeCost;
                PlayerPrefs.SetInt("TotalCoins", QuizGameManager.Instance.coin);
                MapScoreUI scoreUI = FindObjectOfType<MapScoreUI>();
                if (scoreUI != null) scoreUI.UpdateScoreUI();

                state = 1;
                PlayerPrefs.SetInt(themeID + "_Purchased", 1);
                PlayerPrefs.Save();
                RefreshUI();
            }
        }
        else if (state == 1) // 장착
        {
            UnequipAllOtherThemes();

            PlayerPrefs.SetString("SelectedTheme", themeID);
            PlayerPrefs.Save();

            if (QuizGameManager.Instance != null)
                QuizGameManager.Instance.EquipTheme(themeID == "default" ? "" : themeID);

            UpdatePreviewBackground();
            MoveSelectionFrame();

            state = 2;
            RefreshUI();
        }
    }

    // 컨트롤러에서 호출할 수 있는 public 함수
    public void UpdatePreviewFromController()
    {
        UpdatePreviewBackground();
    }

    private void UpdatePreviewBackground()
    {
        if (themeBackgroundImage != null)
        {
            if (themeSprite != null)
            {
                themeBackgroundImage.gameObject.SetActive(true);
                themeBackgroundImage.sprite = themeSprite;
                themeBackgroundImage.rectTransform.anchoredPosition = themePosition;
                if (themeSize != Vector2.zero)
                    themeBackgroundImage.rectTransform.sizeDelta = themeSize;
            }
            else
            {
                themeBackgroundImage.gameObject.SetActive(false);
            }
        }
    }

    private void MoveSelectionFrame()
    {
        if (selectionFrame != null && selectionTarget != null)
        {
            selectionFrame.SetActive(true);
            selectionFrame.transform.SetParent(selectionTarget);
            selectionFrame.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            selectionFrame.transform.SetAsLastSibling();
        }
    }

    private void UnequipAllOtherThemes()
    {
        ThemeItem[] allThemes = transform.parent.GetComponentsInChildren<ThemeItem>();
        foreach (ThemeItem theme in allThemes)
        {
            if (theme != this && theme.state == 2)
            {
                theme.state = 1;
                theme.RefreshUI();
            }
        }
    }
}