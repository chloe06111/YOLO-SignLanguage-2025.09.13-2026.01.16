using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreExit : MonoBehaviour
{
    // 버튼을 클릭하면 실행될 함수입니다.
    public void BackToMainMap()
    {
        if (QuizGameManager.Instance != null)
        {
            // 1. StoreZone에서 저장해뒀던 돌아갈 맵 이름을 가져옵니다.
            string returnSceneName = QuizGameManager.Instance.GetReturnMapScene();

            // 2. 만약 저장된 이름이 있다면 그 맵으로 이동합니다.
            if (!string.IsNullOrEmpty(returnSceneName))
            {
                SceneManager.LoadScene(returnSceneName);
            }
            else
            {
                // 혹시 저장된 이름이 없을 때를 대비해 기본 맵 이름을 적어줍니다.
                SceneManager.LoadScene("MainMapScene");
            }
        }
    }
}