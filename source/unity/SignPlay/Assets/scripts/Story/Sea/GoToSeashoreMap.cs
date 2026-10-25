using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSeashoreMap : MonoBehaviour
{
    [SerializeField] private string seashoreSceneName = "SeashoreMapScene";

    // 바다맵에서 “메인맵으로 돌아가기” 할 때 필요하면 넣어두기(선택)
    [SerializeField] private string returnMapSceneName = "MainMapScene";

    public void Go()
    {
        // ✅ 바다맵 진입 스토리 예약
        if (PlayerPrefs.GetInt("Sea_Intro_Shown", 0) == 0)
        {
            PlayerPrefs.SetInt("Sea_Intro_Pending", 1);
            PlayerPrefs.Save();
        }

        // (선택) 돌아갈 맵 이름 저장
        if (QuizGameManager.Instance != null)
            QuizGameManager.Instance.SetReturnMapScene(returnMapSceneName);  // :contentReference[oaicite:1]{index=1}

        SceneManager.LoadScene(seashoreSceneName);
    }
}
