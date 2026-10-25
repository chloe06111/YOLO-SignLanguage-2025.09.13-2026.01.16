using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlaySceneController : MonoBehaviour
{
    // ---------------------------
    // 싱글톤 (씬 전환 컨트롤러)
    // ---------------------------
    public static PlaySceneController Instance { get; private set; }

    [Header("필수 참조")]
    [Tooltip("현재 플레이어 Transform (인스펙터에 넣는 걸 추천). 비어있으면 태그 Player로 자동 탐색합니다.")]
    public Transform player;

    [Header("Scene Names (set in Inspector)")]
    [Tooltip("기본 맵 씬 이름 (혹시 저장된 씬명이 없을 때 fallback으로 사용)")]
    public string mapSceneName = "MainMapScene";

    [Tooltip("기본 퀴즈 씬 이름 (stageKey가 퀴즈 씬 이름을 직접 들고 있다면 무시해도 됨)")]
    public string defaultQuizSceneName = "QuizScene";

    [Tooltip("옵션: 로딩 화면 씬 이름 (없으면 빈칸 가능)")]
    public string loadingSceneName = "LoadingScene";

    [Header("Timing")]
    [Tooltip("씬 로드 전 기본 딜레이 (연출용)")]
    public float defaultLoadDelay = 2.5f;

    [Header("Events (subscribe UI / VFX)")]
    public UnityEvent<string> OnLoadingStarted = new UnityEvent<string>();
    public UnityEvent OnLoadingFinished = new UnityEvent();
    public UnityEvent<int> OnCountdownTick = new UnityEvent<int>();
    public UnityEvent OnCountdownStarted = new UnityEvent();
    public UnityEvent OnCountdownFinished = new UnityEvent();

    private void Awake()
    {
        // 싱글톤 세팅
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 씬 이동을 관리하므로 유지
        DontDestroyOnLoad(gameObject);

        // 씬 로드 이벤트
        SceneManager.sceneLoaded += PlayOnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= PlayOnSceneLoaded;
    }

    // ----------------------------------------
    // 내부 유틸: player가 없으면 자동으로 찾기
    // ----------------------------------------
    private void EnsurePlayerRef()
    {
        if (player != null) return;

        var p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    // ----------------------------------------
    // 씬 로드 (딜레이 포함)
    // ----------------------------------------
    public IEnumerator PlayLoadWithDelay(string sceneName, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        OnLoadingStarted.Invoke(sceneName);
        SceneManager.LoadScene(sceneName);

        yield return null;
        OnLoadingFinished.Invoke();
    }

    // =========================================================
    //  [핵심 1] 맵 -> 퀴즈 씬 로드 (퀴즈 들어가기 직전에 저장!)
    // =========================================================
    public void PlayLoadQuizScene()
    {
        if (QuizGameManager.Instance == null)
        {
            Debug.LogError("[PlaySceneController] PlayLoadQuizScene failed: QuizGameManager.Instance is null.");
            return;
        }

        // (1) 플레이어 참조 확보
        EnsurePlayerRef();

        // ✅✅✅ (2) 퀴즈 들어가기 '직전' 현재 맵 씬 이름 저장 (맵 복귀 전용!)
        //      -> OK 눌러서 돌아올 때, 이 씬으로 다시 로드하게 됨
        string currentMapSceneName = SceneManager.GetActiveScene().name;
        QuizGameManager.Instance.SetReturnMapScene(currentMapSceneName);

        // ✅✅✅ (3) 퀴즈 들어가기 '직전' 플레이어 좌표 저장
        //      -> 맵 씬으로 돌아오면 PlayerReturnSpawner가 이 좌표로 복귀시킴
        if (player != null)
        {
            QuizGameManager.Instance.SetLastPosition(player.position);
            Debug.Log($"[PlaySceneController] Saved return position: {player.position} / map: {currentMapSceneName}");
        }
        else
        {
            Debug.LogWarning("[PlaySceneController] Player reference is missing. Position won't be saved.");
        }

        // (4) 이제 퀴즈 씬으로 이동
        // - 너 현재 프로젝트에서는 기본적으로 QuizScene으로 가는 구조라서 그대로 둠
        string targetScene = defaultQuizSceneName;

        OnLoadingStarted.Invoke(targetScene);

        if (!string.IsNullOrEmpty(loadingSceneName))
        {
            StartCoroutine(PlayLoadWithDelay(loadingSceneName, 0.1f));
            StartCoroutine(PlayLoadWithDelay(targetScene, defaultLoadDelay));
        }
        else
        {
            StartCoroutine(PlayLoadWithDelay(targetScene, defaultLoadDelay));
        }
    }

    // =========================================================
    //  [핵심 2] QuizEnding(결과) -> 저장된 맵 씬으로 복귀
    // =========================================================
    public void PlayLoadMapSceneFromResult()
    {
        // ✅✅✅ 저장된 "맵 씬 이름"이 있으면 그쪽으로, 없으면 Inspector 기본 맵으로
        string targetMap = mapSceneName;

        if (QuizGameManager.Instance != null)
        {
            string savedMap = QuizGameManager.Instance.GetReturnMapScene();
            if (!string.IsNullOrEmpty(savedMap))
                targetMap = savedMap;
        }

        Debug.Log($"[PlaySceneController] Return to MAP scene: {targetMap}");

        OnLoadingStarted.Invoke(targetMap);
        StartCoroutine(PlayLoadWithDelay(targetMap, defaultLoadDelay));
    }

    // ----------------------------------------
    // 카운트다운 + 로드 (있길래 그대로 유지)
    // ----------------------------------------
    public void PlayStartCountdownAndLoad(string sceneName, float countdownSec, Action onTick = null)
    {
        StartCoroutine(PlayCountdownAndLoadCoroutine(sceneName, countdownSec, onTick));
    }

    private IEnumerator PlayCountdownAndLoadCoroutine(string sceneName, float countdownSec, Action onTick)
    {
        if (countdownSec <= 0f)
        {
            OnCountdownStarted.Invoke();
            OnCountdownFinished.Invoke();
            StartCoroutine(PlayLoadWithDelay(sceneName, 0.1f));
            yield break;
        }

        OnCountdownStarted.Invoke();

        int remaining = Mathf.CeilToInt(countdownSec);

        while (remaining > 0)
        {
            OnCountdownTick.Invoke(remaining);
            onTick?.Invoke();

            yield return new WaitForSeconds(1f);

            remaining--;
        }

        OnCountdownTick.Invoke(0);
        OnCountdownFinished.Invoke();

        StartCoroutine(PlayLoadWithDelay(sceneName, 0.1f));
    }

    private void PlayOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnLoadingFinished.Invoke();
    }

    /// <summary>
    /// 게임 완전 종료 (빌드에서만 종료됨)
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("게임을 종료합니다...");
        Application.Quit();
    }
}
