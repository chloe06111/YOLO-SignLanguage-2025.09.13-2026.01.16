using UnityEngine;
using UnityEngine.UI;

public class StoreUIController : MonoBehaviour
{
    [Header("--- List Panels ---")]
    public GameObject skinList;
    public GameObject themeList;

    [Header("--- Tab Buttons ---")]
    public Image skinTabImage;
    public Image themeTabImage;
    public Sprite tabNormal;
    public Sprite tabSelected;

    [Header("--- Preview Layer ---")]
    public GameObject themeBackground; // 왼쪽 배경 오브젝트

    private void OnEnable()
    {
        // ★ [추가] 상점이 열릴 때 테마와 스킨의 상태를 강제로 새로고침합니다.
        RefreshAllItems();
        ShowSkinTab();
    }

    // 비활성화된 탭에 있는 아이템들도 데이터를 읽어 미리보기를 갱신하게 합니다.
    private void RefreshAllItems()
    {
        // 1. 테마 새로고침 (비활성 상태인 아이템도 포함하여 검색)
        if (themeList != null)
        {
            ThemeItem[] allThemes = themeList.GetComponentsInChildren<ThemeItem>(true);
            foreach (var theme in allThemes)
            {
                // 현재 저장된 테마 ID와 일치하는 아이템을 찾아 미리보기 갱신
                string selectedTheme = PlayerPrefs.GetString("SelectedTheme", "default");
                if (theme.themeID == selectedTheme)
                {
                    theme.UpdatePreviewFromController();
                }
            }
        }

        // 2. 스킨 새로고침
        if (skinList != null)
        {
            ShopItem[] allSkins = skinList.GetComponentsInChildren<ShopItem>(true);
            foreach (var skin in allSkins)
            {
                string selectedSkin = PlayerPrefs.GetString("SelectedPlayer", "default");
                if (skin.itemDisplayName == selectedSkin)
                {
                    skin.UpdatePreviewFromController();
                }
            }
        }
    }

    public void ShowSkinTab()
    {
        if (skinList != null) skinList.SetActive(true);
        if (themeList != null) themeList.SetActive(false);

        if (skinTabImage != null) skinTabImage.sprite = tabSelected;
        if (themeTabImage != null) themeTabImage.sprite = tabNormal;
    }

    public void ShowThemeTab()
    {
        if (skinList != null) skinList.SetActive(false);
        if (themeList != null) themeList.SetActive(true);

        if (skinTabImage != null) skinTabImage.sprite = tabNormal;
        if (themeTabImage != null) themeTabImage.sprite = tabSelected;
    }
}