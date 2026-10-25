using UnityEngine;
using UnityEngine.UI;

public class QuizEndingImageSwitcher : MonoBehaviour
{
    public enum TargetType
    {
        UISpriteImage,  // 캔버스 UI 이미지 (Image 컴포넌트)
        SpriteRenderer  // 게임 씬 안의 오브젝트 (SpriteRenderer 컴포넌트)
    }

    public enum AutoResizeMode
    {
        KeepHeight_FitWidthByAspect,   // (UI용) 높이 유지 + 비율대로 가로 조정
        KeepWidth_FitHeightByAspect,   // (UI용) 가로 유지 + 비율대로 높이 조정
        NativeSize                     // (UI용) 원본 픽셀 크기
    }

    [Header("설정 (Target Type을 먼저 골라주세요!)")]
    [Tooltip("UI(캔버스)인지, 씬 오브젝트(SpriteRenderer)인지 선택")]
    [SerializeField] private TargetType targetType = TargetType.UISpriteImage;

    [Header("연결 (사용하는 것 하나만 연결)")]
    [SerializeField] private Image targetImage;         // UI일 때 연결
    [SerializeField] private SpriteRenderer targetRenderer; // 스프라이트일 때 연결

    [Header("결과별 이미지 설정")]
    [Tooltip("힌트 0~1개 (별 3개)일 때 이미지")]
    [SerializeField] private Sprite bestRankSprite;    // (구 jumpingSprite)

    [Tooltip("힌트 2개 (별 2개)일 때 이미지")]
    [SerializeField] private Sprite normalRankSprite;  // (구 runningSprite)

    [Tooltip("힌트 3개 이상 (별 1개)일 때 이미지")]
    [SerializeField] private Sprite badRankSprite;     // (구 logoSprite)

    [Header("UI 옵션")]
    [SerializeField] private AutoResizeMode resizeMode = AutoResizeMode.KeepHeight_FitWidthByAspect;

    private void Awake()
    {
        // 자동으로 타겟 잡기 (연결 안 했을 때 대비)
        if (targetType == TargetType.SpriteRenderer && targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();

        if (targetType == TargetType.UISpriteImage && targetImage == null)
            targetImage = GetComponent<Image>();
    }

    private void Start()
    {
        // 1. 엔딩용 확정 힌트 개수 가져오기 (매니저가 없으면 0으로 가정)
        int hint = 0;
        if (QuizGameManager.Instance != null)
        {
            hint = QuizGameManager.FinalHintUsedCount;
        }

        Debug.Log($"[ImageSwitcher] 힌트 사용 횟수: {hint}");

        // 2. 개수에 따라 이미지 선택
        Sprite selectedSprite = null;

        if (hint <= 1)
        {
            selectedSprite = bestRankSprite; // 별 3개 (최고)
            Debug.Log("-> 결과: 최고 등급 이미지 (0~1회)");
        }
        else if (hint == 2)
        {
            selectedSprite = normalRankSprite; // 별 2개 (중간)
            Debug.Log("-> 결과: 중간 등급 이미지 (2회)");
        }
        else
        {
            selectedSprite = badRankSprite;  // 별 1개 (노력)
            Debug.Log("-> 결과: 기본 등급 이미지 (3회 이상)");
        }

        // 3. 이미지 적용
        if (selectedSprite != null)
        {
            Apply(selectedSprite);
        }
        else
        {
            Debug.LogWarning("[ImageSwitcher] 해당 등급의 스프라이트가 비어있습니다!");
        }
    }

    private void Apply(Sprite sprite)
    {
        if (targetType == TargetType.SpriteRenderer)
        {
            if (targetRenderer != null)
                targetRenderer.sprite = sprite;
        }
        else // UI Image
        {
            if (targetImage != null)
            {
                targetImage.sprite = sprite;
                ResizeUIToSprite(targetImage, sprite);
            }
        }
    }

    private void ResizeUIToSprite(Image img, Sprite sprite)
    {
        if (img == null || sprite == null) return;
        RectTransform rt = img.rectTransform;

        switch (resizeMode)
        {
            case AutoResizeMode.NativeSize:
                img.SetNativeSize();
                break;

            case AutoResizeMode.KeepHeight_FitWidthByAspect: // 높이 고정, 너비 자동
                {
                    float h = rt.sizeDelta.y;
                    if (sprite.rect.height > 0)
                    {
                        float aspectW = (float)sprite.rect.width / sprite.rect.height;
                        rt.sizeDelta = new Vector2(h * aspectW, h);
                    }
                }
                break;

            case AutoResizeMode.KeepWidth_FitHeightByAspect: // 너비 고정, 높이 자동
                {
                    float w = rt.sizeDelta.x;
                    if (sprite.rect.width > 0)
                    {
                        float aspectH = (float)sprite.rect.height / sprite.rect.width;
                        rt.sizeDelta = new Vector2(w, w * aspectH);
                    }
                }
                break;
        }
    }
}