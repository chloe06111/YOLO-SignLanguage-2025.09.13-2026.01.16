using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreZone : MonoBehaviour
{
    [Header("Store Settings")]
    public string storeSceneName = "StoreScene"; // 이동할 상점 씬 이름

    [Header("UI References")]
    public GameObject pressGuideUI;    // "Press S to Start" 안내 UI
    public GameObject loadingWaitUI;   // "Wait a second..." 로딩 UI

    private bool playerInside = false;

    private void Start()
    {
        // 시작할 때 모든 안내 UI는 꺼둡니다.
        if (pressGuideUI != null) pressGuideUI.SetActive(false);
        if (loadingWaitUI != null) loadingWaitUI.SetActive(false);
    }

    private void Update()
    {
        // 발판 안에 있고 S 키를 누르면 실행
        if (playerInside && Input.GetKeyDown(KeyCode.S))
        {
            EnterStore();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (pressGuideUI) pressGuideUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (pressGuideUI) pressGuideUI.SetActive(false);
        }
    }

    private void EnterStore()
    {
        // 1. 안내 문구는 끄고, 로딩 중(Wait) 문구를 켭니다.
        if (pressGuideUI) pressGuideUI.SetActive(false);
        if (loadingWaitUI) loadingWaitUI.SetActive(true);

        if (QuizGameManager.Instance != null)
        {
            // 2. 돌아올 맵 이름과 현재 위치를 저장합니다.
            string currentMap = SceneManager.GetActiveScene().name;
            QuizGameManager.Instance.SetReturnMapScene(currentMap);

            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) QuizGameManager.Instance.SetLastPosition(p.transform.position);
        }

        // 3. 상점 씬으로 이동합니다.
        SceneManager.LoadScene(storeSceneName);
    }
}