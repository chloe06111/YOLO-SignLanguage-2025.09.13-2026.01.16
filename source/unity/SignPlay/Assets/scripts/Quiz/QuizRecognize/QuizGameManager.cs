using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic; // 리스트/딕셔너리 사용 필수
using UnityEngine.SceneManagement;

// 저장할 데이터 구조
[System.Serializable]
public class SimpleSaveData
{
    public int coin;
    public string equippedThemeID = "";
    public List<StageRecord> stageRecords = new List<StageRecord>();
}

[System.Serializable]
public class StageRecord
{
    public string stageKey;
    public int bestStars;
}

public class QuizGameManager : MonoBehaviour
{
    public static QuizGameManager Instance { get; private set; }

    // =========================================================
    // ✅ [복구] 외부 스크립트(UI 등)에서 참조하던 변수들 복구
    // =========================================================
    public static int FinalHintUsedCount = 0;
    public static int EarnedStarsInThisStage = 0;
    public static int EarnedCoinsInThisStage = 0;

    // 에러 해결 1: QuizEndingResultUI에서 찾던 변수 복구
    public static int AddedScoreInThisStage = 0;

    [Header("User Progress")]
    public int coin = 0;

    // 에러 해결 2: MapScoreUI, GameDataResetter에서 찾던 'score' 변수 복구
    // (내부적으로는 별 개수 합계를 여기에 업데이트해줄 겁니다)
    public int score = 0;

    // =========================================================

    [Header("Quiz Data")]
    public string[] quizWords;
    public int CurrentQuizIndex { get; private set; } = 0;
    public int TotalQuizCount => (quizWords == null) ? 0 : quizWords.Length;

    // 메모리 상에서 관리할 스테이지 기록
    private Dictionary<string, int> stageBestStars = new Dictionary<string, int>();

    [Header("Game Settings")]
    public int coinPerStar = 100;

    [Header("Current Status")]
    public int hintUsedCount = 0;
    public string CurrentThemeID = "";
    [SerializeField] private string selectedStageKey = "DefaultStage";

    private string saveFilePath;

    [Header("Map & Return")]
    [SerializeField] private Vector3 lastMapPosition = Vector3.zero;
    [SerializeField] private bool hasLastMapPosition = false;
    [SerializeField] private string returnMapSceneKey = null;

    [Header("Pause State")]
    [SerializeField] private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } private set { isPaused = value; } }

    public static event Action OnPaused;
    public static event Action OnResumed;

    [SerializeField] private string quizEndingSceneName = "QuizEnding";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile_simple.json");
        LoadGameData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void SetQuizWords(string[] newWords)
    {
        if (newWords != null && newWords.Length > 0)
            this.quizWords = (string[])newWords.Clone();
    }

    public void UseHint() { hintUsedCount++; }

    public void NextQuiz()
    {
        CurrentQuizIndex++;

        if (CurrentQuizIndex >= TotalQuizCount)
        {
            CurrentQuizIndex = TotalQuizCount;
            ProcessQuizResult();

            if (IsPaused) Resume();
            StartCoroutine(MoveToEndingSequence());
            return;
        }
    }

    private IEnumerator MoveToEndingSequence()
    {
        var cameraSender = FindObjectOfType<UnityCameraSender>();
        if (cameraSender != null)
        {
            bool isStopped = false;
            cameraSender.StopCamera(() => { isStopped = true; });
            float timer = 0f;
            while (!isStopped && timer < 2.0f) { timer += Time.unscaledDeltaTime; yield return null; }
        }
        SceneManager.LoadScene(quizEndingSceneName);
    }

    private void ProcessQuizResult()
    {
        FinalHintUsedCount = hintUsedCount;

        // 1. 이번 판 별 개수 계산
        int currentStars = (hintUsedCount <= 1) ? 3 : (hintUsedCount == 2 ? 2 : 1);
        EarnedStarsInThisStage = currentStars;

        // =================================================================
        // ✅ [수정] 기존 기록과 비교하여 '추가된 점수(AddedScore)'만 계산
        // =================================================================
        int oldBestStars = 0;
        if (stageBestStars.ContainsKey(selectedStageKey))
        {
            oldBestStars = stageBestStars[selectedStageKey];
        }

        // 추가된 점수 = 이번 점수 - 옛날 최고 점수
        int addedScore = currentStars - oldBestStars;

        // 점수가 더 낮거나 같으면 추가 점수는 0 (음수 방지)
        if (addedScore < 0) addedScore = 0;

        // UI에 표시할 변수에 저장
        AddedScoreInThisStage = addedScore;
        // =================================================================


        // 2. 코인 지급 (코인은 매번 줄지, 최초 클리어만 줄지 기획에 따라 다름. 현재는 매번 지급)
        EarnedCoinsInThisStage = currentStars * coinPerStar;
        coin += EarnedCoinsInThisStage;


        // 3. 신기록 갱신 로직 (메모리상 데이터 업데이트)
        if (!stageBestStars.ContainsKey(selectedStageKey))
        {
            stageBestStars.Add(selectedStageKey, currentStars);
            Debug.Log($"[New Record] 첫 클리어! {selectedStageKey}: {currentStars}별");
        }
        else
        {
            // 기존 기록보다 높을 때만 갱신
            if (currentStars > oldBestStars)
            {
                stageBestStars[selectedStageKey] = currentStars;
                Debug.Log($"[Update Record] 신기록 달성! {selectedStageKey}: {oldBestStars} -> {currentStars}별");
            }
        }

        // ✅ score 변수 업데이트 (총 누적 별 개수)
        score = CalculateTotalStars();

        // 4. 저장
        SaveGameData();
        PlayerPrefs.SetInt(selectedStageKey, 1);
        CheckStoryUnlock();
        PlayerPrefs.Save();
    }

    private void CheckStoryUnlock()
    {
        if (selectedStageKey == "Forest_Quiz_2")
        {
            if (PlayerPrefs.GetInt("Forest_Quiz_1", 0) == 1 && PlayerPrefs.GetInt("Forest_Story12_Shown", 0) == 0)
                PlayerPrefs.SetInt("Forest_Story12_Pending", 1);
        }

        int q3 = PlayerPrefs.GetInt("Forest_Quiz_3", 0);
        int q4 = PlayerPrefs.GetInt("Forest_Quiz_4", 0);
        int q5 = PlayerPrefs.GetInt("Forest_Quiz_5", 0);
        if (PlayerPrefs.GetInt("Forest_Story345_Shown", 0) == 0 && q3 == 1 && q4 == 1 && q5 == 1)
        {
            PlayerPrefs.SetInt("Forest_Story345_Pending", 1);
        }

        //=======
        //Sea 건들지 마라
        //=======

        bool seaQ1Done = (PlayerPrefs.GetInt("Sea_Quiz_1", 0) == 1) || (PlayerPrefs.GetInt("Seashore_Quiz_1", 0) == 1);
        bool seaQ2Done = (PlayerPrefs.GetInt("Sea_Quiz_2", 0) == 1) || (PlayerPrefs.GetInt("Seashore_Quiz_2", 0) == 1);

        bool justClearedSea12 = 
            selectedStageKey == "Sea_Quiz1" || selectedStageKey == "Sea_Quiz_2" || 
            selectedStageKey == "Seashore_Quiz_1" || selectedStageKey == "Seashore_Quiz_2";

        if (justClearedSea12 && seaQ1Done && seaQ2Done && PlayerPrefs.GetInt("Sea_Story12_Shown", 0) == 0)
        {
            PlayerPrefs.SetInt("Sea_Story12_Pending", 1);
        }

        bool seaQ3Done = (PlayerPrefs.GetInt("Sea_Quiz_3", 0) == 1) || (PlayerPrefs.GetInt("Seashore_Quiz_3", 0) == 1);
        bool seaQ4Done = (PlayerPrefs.GetInt("Sea_Quiz_4", 0) == 1) || (PlayerPrefs.GetInt("Seashore_Quiz_4", 0) == 1);
        bool seaQ5Done = (PlayerPrefs.GetInt("Sea_Quiz_5", 0) == 1) || (PlayerPrefs.GetInt("Seashore_Quiz_5", 0) == 1);

        if (PlayerPrefs.GetInt("Sea_Story345_Shown", 0) == 0 && seaQ3Done && seaQ4Done && seaQ5Done)
        {
            PlayerPrefs.SetInt("Sea_Story345_Pending", 1);
        }
    }

    public void SaveGameData()
    {
        if (string.IsNullOrEmpty(saveFilePath))
            saveFilePath = Path.Combine(Application.persistentDataPath, "savefile_simple.json");

        SimpleSaveData data = new SimpleSaveData();
        data.coin = coin;
        data.equippedThemeID = CurrentThemeID;

        foreach (var pair in stageBestStars)
        {
            data.stageRecords.Add(new StageRecord { stageKey = pair.Key, bestStars = pair.Value });
        }

        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data, true));
    }

    public void LoadGameData()
    {
        if (string.IsNullOrEmpty(saveFilePath))
            saveFilePath = Path.Combine(Application.persistentDataPath, "savefile_simple.json");

        if (!File.Exists(saveFilePath)) return;

        try
        {
            SimpleSaveData data = JsonUtility.FromJson<SimpleSaveData>(File.ReadAllText(saveFilePath));
            if (data != null)
            {
                coin = data.coin;
                CurrentThemeID = data.equippedThemeID;

                stageBestStars.Clear();
                foreach (var record in data.stageRecords)
                {
                    if (!stageBestStars.ContainsKey(record.stageKey))
                        stageBestStars.Add(record.stageKey, record.bestStars);
                }

                // ✅ [중요] 불러온 데이터로 score 변수 동기화
                score = CalculateTotalStars();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Load Error] 데이터 로드 실패: {e.Message}");
        }
    }

    // --- 유틸리티 함수들 ---

    public int GetStarCount(string stageKey)
    {
        if (stageBestStars.ContainsKey(stageKey))
            return stageBestStars[stageKey];
        return 0;
    }

    private int CalculateTotalStars()
    {
        int total = 0;
        foreach (var stars in stageBestStars.Values)
        {
            total += stars;
        }
        return total;
    }

    public void EquipTheme(string themeID)
    {
        CurrentThemeID = themeID;
        SaveGameData();
    }

    public void Pause()
    {
        if (!IsPaused) { IsPaused = true; Time.timeScale = 0f; OnPaused?.Invoke(); }
    }

    public void Resume()
    {
        if (IsPaused) { IsPaused = false; Time.timeScale = 1f; OnResumed?.Invoke(); }
    }

    public string GetProgressText()
    {
        return $"{Mathf.Clamp(CurrentQuizIndex + 1, 1, TotalQuizCount)}/{TotalQuizCount}";
    }

    public void SetLastPosition(Vector3 pos) { lastMapPosition = pos; hasLastMapPosition = true; }

    public bool GetLastMapPosition(out Vector3 pos)
    {
        if (hasLastMapPosition) { pos = lastMapPosition; hasLastMapPosition = false; return true; }
        pos = Vector3.zero; return false;
    }

    public void SelectStage(string key) { selectedStageKey = key; }
    public string GetSelectedStageKey() { return selectedStageKey; }

    public void SetReturnMapScene(string sceneName) { returnMapSceneKey = sceneName; }
    public string GetReturnMapScene() { return returnMapSceneKey; }

    public void ResetQuizSession()
    {
        CurrentQuizIndex = 0;
        hintUsedCount = 0;
        FinalHintUsedCount = 0;
        EarnedStarsInThisStage = 0;
        EarnedCoinsInThisStage = 0;

        // ✅ UI 표시용 변수 초기화
        AddedScoreInThisStage = 0;
    }

    // ✅ 메모리 상의 모든 데이터를 0으로 만드는 함수
    public void ClearMemoryData()
    {
        // 1. 변수 초기화
        coin = 0;
        score = 0;
        CurrentThemeID = "";
        hintUsedCount = 0;

        // 2. 중요: 딕셔너리(클리어 기록) 싹 비우기
        stageBestStars.Clear();

        // 3. UI 표시용 static 변수들도 초기화
        FinalHintUsedCount = 0;
        EarnedStarsInThisStage = 0;
        EarnedCoinsInThisStage = 0;
        AddedScoreInThisStage = 0;

        Debug.Log("<color=red>[QuizGameManager] 메모리 데이터가 완벽하게 초기화되었습니다.</color>");
    }
}