using UnityEngine;
using System.IO;       // 파일 수정용
using UnityEngine.UI;  // 버튼 연결용

public class DeveloperModeStandalone : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject devPanel; // 개발자 메뉴 패널

    [Header("Settings")]
    public int coinToAddAmount = 1000; // 한 번에 추가할 코인

    // 저장 데이터 구조 (QuizGameManager의 것과 똑같이 맞춰야 깨지지 않음)
    [System.Serializable]
    private class SaveDataMirror
    {
        public int score;
        public int coin;
        public string equippedThemeID; // 아까 추가한 테마 ID까지 포함
    }

    private string saveFilePath;

    // 잠금 해제할 스테이지 목록
    private string[] allStageKeys = new string[]
    {
        "Forest_Quiz_1", "Forest_Quiz_2", "Forest_Quiz_3", "Forest_Quiz_4", "Forest_Quiz_5",
        "Seashore_Quiz_1", "Seashore_Quiz_2", "Seashore_Quiz_3", "Seashore_Quiz_4", "Seashore_Quiz_5",
        "Sea_Quiz_1", "Sea_Quiz_2", "Sea_Quiz_3", "Sea_Quiz_4", "Sea_Quiz_5"
    };

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile_simple.json");

        if (devPanel != null) devPanel.SetActive(false);
    }

    // 1. 패널 열기/닫기
    public void OnClickToggle()
    {
        if (devPanel != null) devPanel.SetActive(!devPanel.activeSelf);
    }

    // 2. [파일 직접 수정] 코인 추가
    public void OnClickAddCoinDirectly()
    {
        SaveDataMirror data = new SaveDataMirror();

        // (1) 파일이 이미 있으면 불러오기
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            data = JsonUtility.FromJson<SaveDataMirror>(json);
        }

        // (2) 코인 추가
        data.coin += coinToAddAmount;

        // (3) 다시 JSON으로 변환해서 저장
        string newJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, newJson);

        Debug.Log($"[Dev] 파일 직접 수정 완료! 현재 코인(파일): {data.coin}");
    }

    // 3. [PlayerPrefs 직접 수정] 올클리어
    public void OnClickUnlockAll()
    {
        // PlayerPrefs는 매니저 없어도 언제든 접근 가능
        foreach (string key in allStageKeys)
        {
            PlayerPrefs.SetInt(key, 1);
        }

        // 스토리 해제
        PlayerPrefs.SetInt("Forest_Story12_Shown", 1);
        PlayerPrefs.SetInt("Forest_Story345_Shown", 1);
        PlayerPrefs.SetInt("Sea_Story12_Shown", 1);
        PlayerPrefs.SetInt("Sea_Story345_Shown", 1);

        // ★ 중요: 바뀐 내용을 디스크에 즉시 반영
        PlayerPrefs.Save();

        Debug.Log("[Dev] 모든 스테이지 및 스토리 잠금 해제 (PlayerPrefs 저장됨)");
    }

    // 4. [완전 삭제] 데이터 초기화
    public void OnClickResetData()
    {
        // JSON 파일 삭제
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("[Dev] JSON 세이브 파일 삭제됨");
        }

        // PlayerPrefs 삭제
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("[Dev] 모든 데이터가 초기화되었습니다. (완전 처음 상태)");
    }
}