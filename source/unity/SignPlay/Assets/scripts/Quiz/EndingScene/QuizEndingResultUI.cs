using UnityEngine;
using TMPro;

public class QuizEndingResultUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI earnedCoinText;    // 코인 (예: +300)
    public TextMeshProUGUI totalProgressText; // 전체 진행도 (예: 5 / 30)
    public TextMeshProUGUI addedStarText;     // ✅ 추가된 별 점수 (예: +2)

    [Header("Settings")]
    public int maxStarCount = 30;

    private void Start() { UpdateResultUI(); }
    private void OnEnable() { UpdateResultUI(); }

    private void UpdateResultUI()
    {
        if (QuizGameManager.Instance == null) return;

        // 1. 코인 표시
        if (earnedCoinText != null)
        {
            earnedCoinText.text = $"+{QuizGameManager.EarnedCoinsInThisStage}";
        }

        // 2. 전체 진행도 표시
        if (totalProgressText != null)
            totalProgressText.text = $"{QuizGameManager.Instance.score} / {maxStarCount}";

        // =======================================================
        // 3. ✅ 추가된 별 점수 표시 및 색상 변경 (#4a4a4a)
        // =======================================================
        if (addedStarText != null)
        {
            int added = QuizGameManager.AddedScoreInThisStage;

            // #4a4a4a 색상 생성
            Color customDarkGray;
            ColorUtility.TryParseHtmlString("#4a4a4a", out customDarkGray);

            if (added > 0)
            {
                addedStarText.text = $"+{added}";
                // 기존 노란색 대신 원하시는 #4a4a4a 적용
                addedStarText.color = customDarkGray;
            }
            else
            {
                addedStarText.text = "+0";
                // +0일 때도 같은 색을 원하시면 customDarkGray, 
                // 흐리게 하고 싶으면 Color.gray를 쓰시면 됩니다. 
                // 여기서는 통일감을 위해 같은 색(#4a4a4a)이나 회색 중 선택하시면 됩니다.
                addedStarText.color = Color.gray;
            }
        }
    }
}