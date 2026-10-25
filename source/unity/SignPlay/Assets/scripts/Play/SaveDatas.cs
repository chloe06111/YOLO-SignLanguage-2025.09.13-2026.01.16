using UnityEngine;
using System.Collections.Generic;

// 각 스테이지의 최고 기록을 담는 작은 그릇
[System.Serializable]
public class StageData
{
    public string stageKey; // 스테이지 ID (예: "Forest_Quiz_1")
    public int bestStars;   // 여기서 딴 최고 별 개수 (1~3)
}

[System.Serializable]
public class SaveDatas
{
    public int score; // 총 별 개수 (보여주기용, 진행도)
    public int coin;  // ✅ 보유 코인

    public string equippedThemeID = "";

    // ✅ 스테이지별 기록 리스트 (신기록 판별용)
    public List<StageData> stageRecords = new List<StageData>();
}