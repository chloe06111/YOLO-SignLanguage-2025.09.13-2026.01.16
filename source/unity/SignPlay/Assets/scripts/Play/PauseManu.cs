using UnityEngine;

// 이 스크립트는 'PauseMenu_Prefab_Canvas' 같은 최상위 오브젝트에 붙습니다.
public class PauseMenu : MonoBehaviour
{
    // (중요!) Inspector 창에 'PauseMenuPanel'을 끌어다 놓을 빈칸입니다.
    public CanvasGroup pauseMenuCanvasGroup;

    private void Start()
    {
        if (QuizGameManager.Instance == null) return;

        QuizGameManager.OnPaused += Show;
        QuizGameManager.OnResumed += Hide;

        // 시작할 땐 무조건 숨김
        Hide();
    }

    private void OnDestroy()
    {
        if (QuizGameManager.Instance == null) return;
        QuizGameManager.OnPaused -= Show;
        QuizGameManager.OnResumed -= Hide;
    }

    // CanvasGroup을 켜는 함수
    public void Show()
    {
        if (pauseMenuCanvasGroup == null) return;
        pauseMenuCanvasGroup.alpha = 1f;
        pauseMenuCanvasGroup.interactable = true;
        pauseMenuCanvasGroup.blocksRaycasts = true;
    }

    // CanvasGroup을 끄는 함수
    public void Hide()
    {
        if (pauseMenuCanvasGroup == null) return;
        pauseMenuCanvasGroup.alpha = 0f;
        pauseMenuCanvasGroup.interactable = false;
        pauseMenuCanvasGroup.blocksRaycasts = false;
    }

    // --- [수정됨] UI 버튼에 연결할 함수들 ---

    /// <summary>
    /// '계속하기(Resume)' 버튼에 이 함수를 연결
    /// </summary>
    public void OnClick_Resume()
    {
        if (QuizGameManager.Instance != null)
        {
            QuizGameManager.Instance.Resume(); // 시간 다시 흐르게
            QuizGameManager.Instance.SaveGameData();
        }
    }

    /// <summary>
    /// '게임 종료(Quit Game)' 버튼에 이 함수를 연결
    /// </summary>
    public void OnClick_QuitGame()
    {
        Debug.Log("[PauseMenu] 게임 종료 버튼 클릭됨");

        if (QuizGameManager.Instance != null)
        {
            QuizGameManager.Instance.Resume(); // 멈춘 시간 다시 흐르게
            QuizGameManager.Instance.SaveGameData(); // 데이터 저장
        }

        // 1. 카메라 끄기
        var cameraSender = FindObjectOfType<UnityCameraSender>();
        if (cameraSender != null)
        {
            cameraSender.StopCamera();
        }

        // 🔥 [핵심 추가] 여기서 파이썬 서버의 숨통을 확실히 끊습니다! 🔥
        if (ProcessStarter.Instance != null)
        {
            ProcessStarter.Instance.KillCurrentProcess();
        }
        else
        {
            Debug.LogWarning("ProcessStarter 인스턴스를 찾을 수 없습니다.");
        }

        // 2. 게임 종료 (PlaySceneController에게 위임하거나 직접 종료)
        var loader = FindObjectOfType<PlaySceneController>();
        if (loader != null)
            loader.QuitGame();
        else
            Application.Quit();
    }
}