using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizZone : MonoBehaviour
{
    [Header("Stage Settings")]
    public string stageKey = "Forest_Quiz_1";  // ❗ 구역마다 다르게 설정하세요! (기록용 ID)

    // ✅ 이동할 "퀴즈 씬" 이름이어야 합니다.
    public string quizSceneName = "QuizScene";

    [Header("Quiz Words (5 words)")]
    public string[] specificQuizWords; // ✅ 여기서 단어를 설정합니다.

    [Header("UI References")]
    public GameObject pressGuideUI;
    public GameObject loadingWaitUI;

    private bool playerInside = false;
    private bool isLoadingStarted = false;

    private void Start()
    {
        if (pressGuideUI != null) pressGuideUI.SetActive(false);
        if (loadingWaitUI != null) loadingWaitUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInside && !isLoadingStarted && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(PlayInteractionSequence());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { playerInside = true; if (pressGuideUI) pressGuideUI.SetActive(true); }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { playerInside = false; if (pressGuideUI) pressGuideUI.SetActive(false); }
    }

    private IEnumerator PlayInteractionSequence()
    {
        isLoadingStarted = true;
        if (pressGuideUI) pressGuideUI.SetActive(false);
        if (loadingWaitUI) loadingWaitUI.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        if (QuizGameManager.Instance != null)
        {
            // 1. 단어 전달
            if (specificQuizWords != null && specificQuizWords.Length > 0)
                QuizGameManager.Instance.SetQuizWords(specificQuizWords);

            // 2. 초기화 및 설정
            QuizGameManager.Instance.ResetQuizSession();
            QuizGameManager.Instance.SelectStage(stageKey); // ID 설정

            // 3. 복귀 정보 저장 (✅ 현재 맵 이름 저장 → QuizScene에서 배경 판단 가능)
            string currentMap = SceneManager.GetActiveScene().name;
            QuizGameManager.Instance.SetReturnMapScene(currentMap);

            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) QuizGameManager.Instance.SetLastPosition(p.transform.position);
        }

        // 씬 로드
        if (PlaySceneController.Instance != null)
            StartCoroutine(PlaySceneController.Instance.PlayLoadWithDelay(quizSceneName, 0f));
        else
            SceneManager.LoadScene(quizSceneName);
    }
}
