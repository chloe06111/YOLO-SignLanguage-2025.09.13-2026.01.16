using UnityEngine;
using UnityEngine.UI;

public class StarsAppear : MonoBehaviour
{
    [Header("Star UI Images (Drag Star1/Star2/Star3 Image components here)")]
    public Image star1;   // 왼쪽 별
    public Image star2;   // 가운데 별
    public Image star3;   // 오른쪽 별

    [Header("Debug")]
    public bool showDebugLog = true;

    private void Start()
    {
        // 1) 별 레퍼런스 누락 체크
        if (star1 == null || star2 == null || star3 == null)
        {
            Debug.LogError("[StarsAppear] Star1/Star2/Star3(Image)가 Inspector에 연결되지 않았어요!");
            return;
        }

        // 2) ⭐ 엔딩 씬에서는 확정된 힌트 값만 사용
        int hintCount = QuizGameManager.FinalHintUsedCount;

        // 3) 우선 전부 끄기 (Image는 enabled로 제어)
        SetStars(0);

        // 4) 규칙대로 별 개수 결정
        // 0~1회: 3개, 2회: 2개, 3회 이상: 1개
        int starCount;
        if (hintCount <= 1) starCount = 3;
        else if (hintCount == 2) starCount = 2;
        else starCount = 1;

        // 5) 별 표시
        SetStars(starCount);

        // 6) 디버그 로그
        if (showDebugLog)
            Debug.Log($"[StarsAppear] FinalHintUsedCount={hintCount} → stars={starCount}");
    }

    /// <summary>
    /// starCount 만큼 왼쪽부터 별을 켠다. (0~3)
    /// </summary>
    private void SetStars(int starCount)
    {
        // 항상 초기화 후 켜기
        star1.enabled = false;
        star2.enabled = false;
        star3.enabled = false;

        if (starCount >= 1) star1.enabled = true;
        if (starCount >= 2) star2.enabled = true;
        if (starCount >= 3) star3.enabled = true;
    }
}
