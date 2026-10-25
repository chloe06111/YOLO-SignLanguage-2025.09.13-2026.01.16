using UnityEngine;

// 씬이 바뀌어도 날아가지 않는, 단순 데이터 저장용 static 클래스
public static class GameData
{
    public static Vector3 lastMapPosition;  // 맵에서 마지막 플레이어 위치
    public static bool hasLastMapPosition = false; // 저장된 위치가 있는지 표시
}
