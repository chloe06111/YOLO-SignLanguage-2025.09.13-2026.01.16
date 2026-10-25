using UnityEngine;

public class SeashoreLevelManager : MonoBehaviour
{
    [Header("--- 1단계: 조건이 되는 퀴즈들 (항상 열림) ---")]
    public string quiz1Key = "Seashore_Quiz_1";
    public string quiz2Key = "Seashore_Quiz_2";

    [Header("--- 2단계: 잠겨있어야 할 퀴즈 오브젝트들 ---")]
    public GameObject[] lockedQuizzes;

    [Header("--- 2단계: 잠겨있어야 할 점선 길 (선택사항) ---")]
    public GameObject[] lockedPaths;

    [Header("--- 잠금 표시 설정 ---")]
    public Color lockedColor = Color.gray;
    public Color unlockedColor = Color.white;

    void Start()
    {
        Invoke("CheckAndUnlockQuizzes", 0.1f);
    }

    public void CheckAndUnlockQuizzes()
    {
        bool isQuiz1Clear = PlayerPrefs.GetInt(quiz1Key, 0) == 1;
        bool isQuiz2Clear = PlayerPrefs.GetInt(quiz2Key, 0) == 1;

        if (isQuiz1Clear && isQuiz2Clear)
        {
            UnlockContent();
        }
        else
        {
            LockContent();
        }
    }

    void LockContent()
    {
        foreach (GameObject quiz in lockedQuizzes)
        {
            if (quiz != null)
            {
                if (quiz.GetComponent<Collider2D>() != null) quiz.GetComponent<Collider2D>().enabled = false;
                if (quiz.GetComponent<SpriteRenderer>() != null) quiz.GetComponent<SpriteRenderer>().color = lockedColor;
            }
        }
        foreach (GameObject path in lockedPaths)
        {
            ChangePathColor(path, lockedColor);
        }
    }

    void UnlockContent()
    {
        foreach (GameObject quiz in lockedQuizzes)
        {
            if (quiz != null)
            {
                if (quiz.GetComponent<Collider2D>() != null) quiz.GetComponent<Collider2D>().enabled = true;
                if (quiz.GetComponent<SpriteRenderer>() != null) quiz.GetComponent<SpriteRenderer>().color = unlockedColor;
            }
        }

        foreach (GameObject path in lockedPaths)
        {
            ChangePathColor(path, unlockedColor);
        }
    }


    void ChangePathColor(GameObject pathParent, Color color)
    {
        if (pathParent == null) return;
        SpriteRenderer[] dots = pathParent.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer dot in dots)
        {
            dot.color = color;
        }
    }

    [ContextMenu("테스트: 바다 퀴즈 1,2 클리어")]
    public void TestClearQuizzes()
    {
        PlayerPrefs.SetInt(quiz1Key, 1);
        PlayerPrefs.SetInt(quiz2Key, 1);
        PlayerPrefs.Save();
        Debug.Log("바다 맵 클리어 상태로 변경됨!");
    }

    [ContextMenu("테스트: 데이터 리셋")]
    public void TestResetData()
    {
        PlayerPrefs.DeleteKey(quiz1Key);
        PlayerPrefs.DeleteKey(quiz2Key);
        Debug.Log("바다 맵 데이터 리셋됨!");
    }
}