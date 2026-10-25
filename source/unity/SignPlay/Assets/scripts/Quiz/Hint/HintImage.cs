using UnityEngine;

public class HintImage : MonoBehaviour
{
    public GameObject hintPopup;
    private bool isShown = false;

    public void ToggleHint()
    {
        isShown = !isShown;

        // 패널 활성화 / 비활성화
        if (hintPopup != null)
            hintPopup.SetActive(isShown);

        // 힌트를 "켜는 순간"만 카운트 증가
        if (isShown && QuizGameManager.Instance != null)
        {
            QuizGameManager.Instance.UseHint();
            Debug.Log("힌트 사용 횟수: " + QuizGameManager.Instance.hintUsedCount);
        }
    }
}
