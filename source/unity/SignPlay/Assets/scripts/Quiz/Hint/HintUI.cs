using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HintUI : MonoBehaviour
{
    [Header("Hook in Inspector")]
    [SerializeField] private Button hintButton;        // HintButton 연결
    [SerializeField] private CanvasGroup hintGroup;    // HintPanel의 CanvasGroup 연결
    [SerializeField] private Image hintImage;          // HintPanel의 Image 연결
    [SerializeField] private Sprite hintSprite;        // (선택) 다른 힌트 스프라이트를 코드로 지정
    [SerializeField] private TextMeshProUGUI buttonLabel; // 버튼 텍스트 (선택)
    [SerializeField] private bool startHidden = true;
    [SerializeField] private float fadeDuration = 0.15f;

    private bool shown = false;

    private void Awake()
    {
        // 스프라이트를 코드에서 지정하고 싶다면 Inspector에 hintSprite 할당
        if (hintImage != null && hintSprite != null)
            hintImage.sprite = hintSprite;

        if (startHidden) HideInstant();

        if (hintButton != null)
            hintButton.onClick.AddListener(ToggleHint);
    }

    private void ToggleHint()
    {
        shown = !shown;
        StopAllCoroutines();
        StartCoroutine(Fade(hintGroup.alpha, shown ? 1f : 0f));

        if (buttonLabel != null)
            buttonLabel.text = shown ? "Hide Hint" : "Hint";
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        // 보일 때만 상호작용/레이캐스트 허용
        hintGroup.blocksRaycasts = to > 0.5f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            hintGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        hintGroup.alpha = to;
        hintGroup.interactable = to > 0.5f;
        hintGroup.blocksRaycasts = to > 0.5f;
    }

    private void HideInstant()
    {
        hintGroup.alpha = 0f;
        hintGroup.interactable = false;
        hintGroup.blocksRaycasts = false;
        shown = false;
        if (buttonLabel != null) buttonLabel.text = "Hint";
    }
}
