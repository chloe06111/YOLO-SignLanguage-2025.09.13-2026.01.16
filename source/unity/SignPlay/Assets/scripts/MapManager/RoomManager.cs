using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    // static 변수로 문 번호 공유
    public static int doorNumber = 0;

    // Start()에 있던 플레이어 이동 코드는 전부 삭제했습니다!
    // 그 역할은 이제 PlayerReturnSpawner가 담당합니다.

    // 씬 이동 함수 (Exit.cs에서 호출)
    public static void ChangeScene(string scnename, int doornum)
    {
        doorNumber = doornum;
        SceneManager.LoadScene(scnename);
    }
}