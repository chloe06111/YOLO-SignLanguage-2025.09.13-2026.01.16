using UnityEngine;

// 이 스크립트를 쓰면 자동으로 'Exit' 컴포넌트도 같이 추가됩니다.
[RequireComponent(typeof(Exit))]
public class SeashoreLockedExit : MonoBehaviour
{
    [Header("--- 이동 설정 ---")]
    public string sceneName = "SeashoreMapScene"; // 이동할 씬 이름
    public int doorNumber = 2; // 도착할 문 번호 & 돌아올 때의 내 번호

    // 스포너가 플레이어를 배치할 방향
    public ExitDirection direction = ExitDirection.left;

    [Header("--- 해금 조건 (숲 퀴즈 3,4,5) ---")]
    public string key3 = "Forest_Quiz_3";
    public string key4 = "Forest_Quiz_4";
    public string key5 = "Forest_Quiz_5";

    // 내부적으로 사용할 Exit 컴포넌트 참조
    private Exit myExitComponent;

    private void Awake()
    {
        // 1. 내 오브젝트에 붙어 있는 Exit 컴포넌트를 가져옵니다.
        myExitComponent = GetComponent<Exit>();

        if (myExitComponent != null)
        {
            // 2. 기본 Exit 기능은 끕니다. (조건 검사 없이 이동하면 안 되니까)
            myExitComponent.enabled = false;

            // 3. [핵심] 내 설정값을 Exit 컴포넌트에 복사해줍니다.
            // 이렇게 해야 'PlayerReturnSpawner'가 이 문을 2번 문으로 인식하고 플레이어를 여기로 데려옵니다.
            myExitComponent.doorNumber = this.doorNumber;
            myExitComponent.direction = this.direction;
            myExitComponent.sceneName = this.sceneName;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어인지 확인
        if (collision.CompareTag("Player"))
        {
            // 1. 숲 퀴즈 3, 4, 5번을 모두 깼는지 검사
            if (CheckForestClear())
            {
                Debug.Log("조건 만족! 바다로 이동합니다.");

                // 2. 바다 맵 첫 진입 시 스토리 재생 예약
                if (sceneName == "SeashoreMapScene" && PlayerPrefs.GetInt("Sea_Intro_Shown", 0) == 0)
                {
                    PlayerPrefs.SetInt("Sea_Intro_Pending", 1);
                    PlayerPrefs.Save();
                }

                // 3. (중요) 돌아올 때를 위해 '현재 맵 이름'을 퀴즈 매니저에 저장
                if (QuizGameManager.Instance != null)
                {
                    // 돌아올 때 MainMapScene으로 오도록 저장
                    QuizGameManager.Instance.SetReturnMapScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                }

                // 4. 씬 이동 (문 번호 전달)
                RoomManager.ChangeScene(sceneName, doorNumber);
            }
            else
            {
                Debug.Log("잠겨있음: 숲의 퀴즈 3, 4, 5번을 모두 풀어야 합니다!");
            }
        }
    }

    // 조건 확인 함수
    bool CheckForestClear()
    {
        bool q3 = PlayerPrefs.GetInt(key3, 0) == 1;
        bool q4 = PlayerPrefs.GetInt(key4, 0) == 1;
        bool q5 = PlayerPrefs.GetInt(key5, 0) == 1;

        return q3 && q4 && q5;
    }
}