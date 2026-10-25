using UnityEngine;

public class ForestLevelManager : MonoBehaviour
{
    [Header("--- 1단계: 조건이 되는 퀴즈들 (항상 열림) ---")]
    public string quiz1Key = "Forest_Quiz_1";
    public string quiz2Key = "Forest_Quiz_2";

    [Header("--- 2단계: 잠겨있어야 할 퀴즈 오브젝트들 ---")]
    public GameObject[] lockedQuizzes;

    [Header("--- 잠금 표시 설정 ---")]
    public Color lockedColor = Color.gray;
    public Color unlockedColor = Color.white;

    void Start()
    {
        CheckAndUnlockQuizzes();
    }

    public void CheckAndUnlockQuizzes()
    {
        bool isQuiz1Clear = IsCleared(quiz1Key);
        bool isQuiz2Clear = IsCleared(quiz2Key);

        if (isQuiz1Clear && isQuiz2Clear)
        {
            UnlockQuizzes();
        }
        else
        {
            LockQuizzes();
        }
    }

    // ✅ PlayerPrefs 기준으로만 검사 (stageRecords 제거)
    private bool IsCleared(string key)
    {
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    void LockQuizzes()
    {
        foreach (GameObject quiz in lockedQuizzes)
        {
            if (quiz == null) continue;

            var col = quiz.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            var sr = quiz.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = lockedColor;
        }
    }

    void UnlockQuizzes()
    {
        foreach (GameObject quiz in lockedQuizzes)
        {
            if (quiz == null) continue;

            var col = quiz.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;

            var sr = quiz.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = unlockedColor;
        }
    }

    [ContextMenu("테스트: 퀴즈 1,2 클리어 처리")]
    public void TestClearQuizzes()
    {
        PlayerPrefs.SetInt(quiz1Key, 1);
        PlayerPrefs.SetInt(quiz2Key, 1);
        PlayerPrefs.Save();
        Debug.Log("퀴즈 1, 2 클리어 상태로 변경됨! 다시 재생해보세요.");
    }

    [ContextMenu("테스트: 데이터 리셋")]
    public void TestResetData()
    {
        PlayerPrefs.DeleteKey(quiz1Key);
        PlayerPrefs.DeleteKey(quiz2Key);
        PlayerPrefs.Save();
        Debug.Log("데이터 리셋됨! 다시 잠길 것입니다.");
    }
}
