using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // 씬 전환 이벤트를 구독하기 위해 필수입니다.

/// <summary>
/// 배경 음악(BGM) 재생을 전역적으로 관리하는 싱글톤 매니저입니다.
/// - 씬이 전환되어도 음악 재생 상태를 유지합니다.
/// - BGM 전환 시 부드러운 페이드 인/아웃 기능을 제공합니다.
/// - 씬 로드 시 씬 이름에 따라 자동으로 BGM을 변경합니다.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlayBGMManager : MonoBehaviour
{
    // =============================================================
    //  [싱글톤 인스턴스]
    // =============================================================
    public static PlayBGMManager Instance { get; private set; }

    // =============================================================
    //  [필드: Inspector에서 설정] - 총 6개의 BGM 필드
    // =============================================================

    [Header("Default Music Clips")]
    // 이 6개의 필드에 해당하는 AudioClip 파일을 Inspector에 연결해야 합니다.
    public AudioClip startSceneBGM;       // StartScene BGM
    public AudioClip mainMapSceneBGM;     // <--- 새로 추가된 MainMapScene BGM
    public AudioClip seashoreMapSceneBGM; // SeashoreMapScene BGM
    public AudioClip forestMapSceneBGM;   // ForestMapScene BGM
    public AudioClip quizSceneBGM;        // QuizScene BGM
    public AudioClip completeSceneBGM;    // Complete BGM

    [Header("BGM Settings")]
    [Tooltip("기본 BGM 볼륨 (0.0 ~ 1.0)")]
    [SerializeField] private float defaultVolume = 0.5f;
    [Tooltip("BGM 전환 시 페이드 인/아웃에 걸리는 시간")]
    [SerializeField] private float fadeDuration = 1.0f;

    // =============================================================
    //  [내부 상태]
    // =============================================================
    private AudioSource audioSource;
    private Coroutine currentFadeRoutine = null;

    // =============================================================
    //  [초기화 및 씬 이벤트 처리]
    // =============================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // BGM은 기본적으로 반복 재생
        audioSource.volume = defaultVolume;

        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제 (오브젝트가 파괴될 때 필수)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[PlayBGMManager] 씬 로드 완료: {scene.name}");
        PlayChangeBGMForScene(scene.name); // 씬 이름에 따라 BGM 변경 함수 호출
    }

    // =============================================================
    //  [핵심 BGM 제어 함수]
    // =============================================================

    /// <summary>
    /// 6개의 씬 이름에 따라 재생할 BGM 클립을 선택하여 재생합니다.
    /// </summary>
    public void PlayChangeBGMForScene(string sceneName)
    {
        AudioClip targetBGM = null;

        switch (sceneName)
        {
            case "StartScene":
                targetBGM = startSceneBGM;
                break;
            case "MainMapScene": // <--- MainMapScene에 대한 로직 추가
                targetBGM = mainMapSceneBGM;
                break;
            case "SeashoreMapScene":
                targetBGM = seashoreMapSceneBGM;
                break;
            case "ForestMapScene":
                targetBGM = forestMapSceneBGM;
                break;
            case "QuizScene":
                targetBGM = quizSceneBGM;
                break;
            case "Complete":
                targetBGM = completeSceneBGM;
                break;
            default:
                // 지정되지 않은 씬이 로드되면 BGM을 유지합니다.
                Debug.Log($"[PlayBGMManager] 씬 '{sceneName}'에 지정된 BGM이 없어 현재 BGM 유지.");
                return; // 함수 종료
        }

        if (targetBGM != null)
        {
            PlayBGM(targetBGM);
        }
    }

    public void PlayBGM(AudioClip newClip)
    {
        if (newClip == null) return;
        if (audioSource.clip == newClip && audioSource.isPlaying) return;

        if (currentFadeRoutine != null)
        {
            StopCoroutine(currentFadeRoutine);
        }

        currentFadeRoutine = StartCoroutine(PlayFadeAndPlayRoutine(newClip));
    }

    public void PlayPauseBGM()
    {
        audioSource.Pause();
    }

    public void PlayResumeBGM()
    {
        audioSource.UnPause();
    }

    public void PlayStopBGM()
    {
        if (currentFadeRoutine != null)
        {
            StopCoroutine(currentFadeRoutine);
            currentFadeRoutine = null;
        }
        audioSource.Stop();
        audioSource.clip = null;
    }

    public void PlaySetVolumeSmoothly(float targetVolume)
    {
        if (currentFadeRoutine != null)
        {
            StopCoroutine(currentFadeRoutine);
        }
        currentFadeRoutine = StartCoroutine(PlayFadeVolumeRoutine(targetVolume));
    }

    // =============================================================
    //  [코루틴: 부드러운 전환 구현]
    // =============================================================

    private IEnumerator PlayFadeAndPlayRoutine(AudioClip newClip)
    {
        if (audioSource.isPlaying)
        {
            yield return StartCoroutine(PlayFadeVolumeRoutine(0f, fadeDuration * 0.5f));
            audioSource.Stop();
        }

        audioSource.clip = newClip;
        audioSource.volume = 0f;
        audioSource.Play();

        yield return StartCoroutine(PlayFadeVolumeRoutine(defaultVolume, fadeDuration * 0.5f));

        currentFadeRoutine = null;
    }

    private IEnumerator PlayFadeVolumeRoutine(float targetVolume, float duration = -1f)
    {
        float actualDuration = (duration > 0) ? duration : fadeDuration;

        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < actualDuration)
        {
            timer += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, timer / actualDuration);
            audioSource.volume = newVolume;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}