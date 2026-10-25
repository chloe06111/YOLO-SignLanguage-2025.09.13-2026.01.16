using UnityEngine;

public class PlayerReturnSpawner : MonoBehaviour
{
    [Header("필수 참조")]
    public Transform player;             // 실제 플레이어 Transform
    public Transform defaultSpawnPoint;  // 게임 처음 시작할 때 스폰 위치

    private void Start()
    {
        if (player == null)
        {
            // 혹시 인스펙터에 안 넣었을 때 태그로 자동 찾기
            var p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player == null)
        {
            Debug.LogError("[PlayerReturnSpawner] Player 참조를 찾지 못했어요.");
            return;
        }

        // ====================================================
        // 우선순위 1: 퀴즈 풀고 돌아왔을 때 (QuizGameManager)
        // ====================================================
        if (QuizGameManager.Instance != null)
        {
            Vector3 savedPos;

            // "저장된 위치가 있냐"를 성공 여부(true/false)로 판단
            if (QuizGameManager.Instance.GetLastMapPosition(out savedPos))
            {
                player.position = savedPos;
                SnapCameraToPlayer(); // ✅ 텔레포트 직후 카메라도 즉시 스냅
                Debug.Log($"[Spawner] 퀴즈 복귀 위치로 이동: {savedPos}");
                return;
            }
        }

        // ====================================================
        // 우선순위 2: 문을 통해 들어왔을 때 (RoomManager)
        // ====================================================
        if (RoomManager.doorNumber != 0)
        {
            GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
            bool doorFound = false;

            foreach (GameObject exitObj in exits)
            {
                Exit exit = exitObj.GetComponent<Exit>();
                if (exit != null && exit.doorNumber == RoomManager.doorNumber)
                {
                    // 문 위치 찾음
                    float x = exitObj.transform.position.x;
                    float y = exitObj.transform.position.y;

                    // 방향에 따라 위치 보정
                    switch (exit.direction)
                    {
                        case ExitDirection.up: y += 1; break;
                        case ExitDirection.right: x += 1; break;
                        case ExitDirection.down: y -= 1; break;
                        case ExitDirection.left: x -= 1; break;
                    }

                    player.position = new Vector3(x, y, 0);
                    SnapCameraToPlayer(); // ✅ 문 이동도 카메라 스냅
                    Debug.Log($"[Spawner] {RoomManager.doorNumber}번 문 앞으로 이동");

                    // 사용한 문 번호 초기화
                    RoomManager.doorNumber = 0;
                    doorFound = true;
                    return;
                }
            }

            if (!doorFound) Debug.LogWarning($"[Spawner] {RoomManager.doorNumber}번 문을 찾을 수 없습니다.");
        }

        // ====================================================
        // 우선순위 3: 아무것도 해당 안 될 때 (기본 위치)
        // ====================================================
        if (defaultSpawnPoint != null)
        {
            player.position = defaultSpawnPoint.position;
            SnapCameraToPlayer(); // ✅ 기본 스폰도 카메라 스냅
            Debug.Log("[Spawner] 기본 스폰 위치로 이동");
        }
    }

    // 카메라가 첫 프레임에 튀는 현상 방지용
    private void SnapCameraToPlayer()
    {
        if (Camera.main != null)
        {
            Vector3 newCamPos = new Vector3(
                player.position.x,
                player.position.y,
                Camera.main.transform.position.z
            );
            Camera.main.transform.position = newCamPos;
        }
    }
}
