using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [Header("--- UI Sign Settings ---")]
    public Image signImage;
    public Sprite applySprite;
    public Sprite pressedApplySprite;
    public TextMeshProUGUI statusText;

    public string itemDisplayName;

    [Header("--- Accessory Layer Settings ---")]
    public Image accessoryImage;
    public Sprite itemSprite;

    [Header("--- Individual Transform Settings ---")]
    public Vector2 itemPosition;
    public Vector2 itemSize;

    [Header("--- Selection Settings ---")]
    public GameObject selectionFrame;
    public RectTransform selectionTarget;

    [Header("--- Cost Settings ---")]
    public int itemCost = 1000;

    [Header("--- Setup ---")]
    public int initialState = 0; // 기본 캐릭터는 2로 설정
    private int state;

    void OnEnable()
    {
        // 저장된 데이터 로드
        string selectedSkin = PlayerPrefs.GetString("SelectedPlayer", "default");

        if (itemDisplayName == selectedSkin)
        {
            state = 2; // 장착 중
        }
        else if (PlayerPrefs.GetInt(itemDisplayName + "_Purchased", 0) == 1)
        {
            state = 1; // 구매 완료
        }
        else
        {
            state = initialState;
        }

        RefreshUI();

        if (state == 2)
        {
            MoveSelectionFrame();
            UpdateAccessoryPreview(); // 상점 열 때 미리보기 갱신
            if (statusText != null) statusText.text = itemDisplayName;
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
            if (QuizGameManager.Instance != null && QuizGameManager.Instance.coin >= itemCost)
            {
                QuizGameManager.Instance.coin -= itemCost;
                PlayerPrefs.SetInt("TotalCoins", QuizGameManager.Instance.coin);
                MapScoreUI scoreUI = FindObjectOfType<MapScoreUI>();
                if (scoreUI != null) scoreUI.UpdateScoreUI();

                state = 1;
                PlayerPrefs.SetInt(itemDisplayName + "_Purchased", 1);
                PlayerPrefs.Save();
                RefreshUI();
            }
        }
        else if (state == 1) // 장착
        {
            UnequipAllOtherItems();

            PlayerPrefs.SetString("SelectedPlayer", itemDisplayName);
            PlayerPrefs.Save();

            UpdateAccessoryPreview();
            MoveSelectionFrame();
            if (statusText != null) statusText.text = itemDisplayName;

            PlayerSpawnManager spawner = FindObjectOfType<PlayerSpawnManager>();
            if (spawner != null) spawner.ApplySelectedPlayerNow();

            state = 2;
            RefreshUI();
        }
    }

    // 컨트롤러에서 호출할 수 있는 public 함수
    public void UpdatePreviewFromController()
    {
        UpdateAccessoryPreview();
    }

    private void UpdateAccessoryPreview()
    {
        if (accessoryImage != null)
        {
            if (itemSprite != null)
            {
                accessoryImage.gameObject.SetActive(true);
                accessoryImage.sprite = itemSprite;
                accessoryImage.rectTransform.anchoredPosition = itemPosition;
                accessoryImage.rectTransform.sizeDelta = itemSize;
            }
            else
            {
                accessoryImage.gameObject.SetActive(false);
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

    private void UnequipAllOtherItems()
    {
        ShopItem[] allItems = transform.parent.GetComponentsInChildren<ShopItem>();
        foreach (ShopItem item in allItems)
        {
            if (item != this && item.state == 2)
            {
                item.state = 1;
                item.RefreshUI();
            }
        }
    }
}