using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizEndingOkButton : MonoBehaviour
{
    /// <summary>
    /// OK 버튼 클릭 시 호출
    /// - PlaySceneController가 있으면 그걸 통해 '저장된 맵'으로 복귀
    /// - 없으면 QuizGameManager에 저장된 "returnMapSceneKey"로 직접 로드
    /// </summary>
    public void OnClickOK()
    {
        // 1) 씬 컨트롤러가 있으면 가장 깔끔
        if (PlaySceneController.Instance != null)
        {
            PlaySceneController.Instance.PlayLoadMapSceneFromResult();
            return;
        }

        // 2) 혹시 컨트롤러가 없으면 저장된 맵 씬으로 직접 이동
        string fallback = "MainMapScene";

        if (QuizGameManager.Instance != null)
        {
            // ✅✅✅ 이제 selectedStageKey가 아니라 returnMapSceneKey를 사용!
            string savedMap = QuizGameManager.Instance.GetReturnMapScene();
            if (!string.IsNullOrEmpty(savedMap))
                fallback = savedMap;
        }

        SceneManager.LoadScene(fallback);
    }
}
