using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneJump : MonoBehaviour
{
    [Header("Fallback Settings")]
    [SerializeField] private string defaultReturnScene = "MainMapScene";

    [Header("UI Settings")]
    public GameObject waitUI; // "Wait a second..." UI 연결

    private void Awake()
    {
        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(Go);
    }

    public void Go()
    {
        StartCoroutine(ProcessExitSequence());
    }

    private IEnumerator ProcessExitSequence()
    {
        // 1. 대기 UI 켜기
        if (waitUI != null) waitUI.SetActive(true);

        // UI가 그려질 시간 확보 (0.1초)
        yield return new WaitForSecondsRealtime(0.1f);

        // 2. 데이터 정리
        string targetScene = defaultReturnScene;
        if (QuizGameManager.Instance != null)
        {
            QuizGameManager.Instance.ResetQuizSession();
            QuizGameManager.Instance.SelectStage("");
            QuizGameManager.Instance.SaveGameData();

            string savedMap = QuizGameManager.Instance.GetReturnMapScene();
            if (!string.IsNullOrEmpty(savedMap)) targetScene = savedMap;
        }

        // ==================================================
        // 3. ⭐ 카메라 끄기 & 씬 이동 (이벤트 방식) ⭐
        // ==================================================
        var cameraSender = FindObjectOfType<UnityCameraSender>();

        if (cameraSender != null)
        {
            Debug.Log("[SceneJump] 카메라 종료 명령 보냄... (대기 중)");

            // "카메라 다 끄고 나면, 이 { } 안의 코드를 실행해줘!"
            cameraSender.StopCamera(() =>
            {
                Debug.Log("📷 카메라 종료 확인됨! -> 씬 이동 시작");
                MoveToScene(targetScene);
            });
        }
        else
        {
            // 카메라가 없으면 바로 이동
            Debug.Log("[SceneJump] 카메라 없음 -> 즉시 이동");
            MoveToScene(targetScene);
        }
    }

    // 씬 이동 함수 분리
    private void MoveToScene(string sceneName)
    {
        if (PlaySceneController.Instance != null)
        {
            PlaySceneController.Instance.PlayLoadMapSceneFromResult();
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}