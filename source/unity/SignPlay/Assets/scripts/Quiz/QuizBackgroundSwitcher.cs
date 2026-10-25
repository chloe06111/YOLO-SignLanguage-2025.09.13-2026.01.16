using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuizBackgroundSwitcher : MonoBehaviour
{
    [Header("Background Image (UI)")]
    public Image backgroundImage;

    [Header("Default Map Backgrounds (Must Assign!)")]
    public Sprite forestBackground;   // 기본 숲 배경
    public Sprite seashoreBackground; // 기본 바다 배경

    [Header("Configuration")]
    public string defaultThemeID = "Default"; // 이 ID면 무조건 기본 배경 사용
    public string forestMapSceneName = "ForestMapScene";
    public string seashoreMapSceneName = "SeashoreMapScene";

    [System.Serializable]
    public struct ThemeOption
    {
        public string id;
        public Sprite sprite;
    }

    [Header("Purchased Themes")]
    public List<ThemeOption> themeDatabase;

    private void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
    }

    private void Start()
    {
        ApplyBackground();
    }

    public void ApplyBackground()
    {
        if (backgroundImage == null) return;

        Sprite target = null;
        string currentTheme = "";

        if (QuizGameManager.Instance != null)
        {
            currentTheme = QuizGameManager.Instance.CurrentThemeID;
        }

        // 1. "Default"라는 이름이거나 빈 문자열이면 -> 바로 기본 배경 로직으로 점프 (리스트 탐색 스킵)
        bool isDefault = string.IsNullOrEmpty(currentTheme) || currentTheme == defaultThemeID;

        // 2. 디폴트가 아닐 때만 리스트에서 찾아보기
        if (!isDefault)
        {
            foreach (var theme in themeDatabase)
            {
                if (theme.id == currentTheme)
                {
                    target = theme.sprite;
                    break;
                }
            }

            // 리스트에 ID는 있는데 스프라이트가 None인 경우도 있을 수 있으니
            // target이 여전히 null이면 isDefault 취급을 해버릴 수도 있음
        }

        // 3. 타겟 이미지가 결정되지 않았으면 (디폴트이거나, 리스트에 없거나, 이미지가 None이거나)
        if (target == null)
        {
            // 기본값 숲 설정
            target = forestBackground;

            if (QuizGameManager.Instance != null)
            {
                string fromMap = QuizGameManager.Instance.GetReturnMapScene();
                if (!string.IsNullOrEmpty(fromMap) && fromMap == seashoreMapSceneName)
                {
                    target = seashoreBackground;
                }
            }
        }

        // 4. 이미지 적용
        if (target != null)
        {
            backgroundImage.sprite = target;
        }
        else
        {
            // ★ 중요: 만약 기본 배경(forest)조차 연결이 안 되어 있다면?
            // 디버그 로그를 띄워서 알려줍니다.
            Debug.LogWarning("배경을 바꾸려고 했으나 target 이미지가 null입니다. 인스펙터에서 Forest Background가 연결되어 있는지 확인하세요.");
        }
    }
}