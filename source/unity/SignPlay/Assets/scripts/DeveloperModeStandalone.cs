using UnityEngine;
using System.IO;       // 파일 수정용
using UnityEngine.UI;  // 버튼 연결용

public class SimpleDevCheat : MonoBehaviour
{
    [Header("Cheat Settings")]
    public int coinToAdd = 99999; // 한 번 누르면 추가될 코인 양

    // 해제할 스테이지 목록 (프로젝트에 맞게 추가/삭제 가능)
    private string[] allStageKeys = new string[]
    {
        "Forest_Quiz_1", "Forest_Quiz_2", "Forest_Quiz_3", "Forest_Quiz_4", "Forest_Quiz_5",
        "Seashore_Quiz_1", "Seashore_Quiz_2", "Seashore_Quiz_3", "Seashore_Quiz_4", "Seashore_Quiz_5",
        "Sea_Quiz_1", "Sea_Quiz_2", "Sea_Quiz_3", "Sea_Quiz_4", "Sea_Quiz_5"
    };

    // 저장 데이터 구조 (QuizGameManager의 SimpleSaveData와 동일해야 함)
    [System.Serializable]
    private class SaveDataMirror
    {
        public int score;
        public int coin;
        public string equippedThemeID; // 테마 정보
    }

    private string saveFilePath;

    private void Awake()
    {
        // 저장 경로 설정
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile_simple.json");
    }

    // ★ 이 함수를 버튼에 연결하세요!
    public void OnClickCheatButton()
    {
        Debug.LogWarning("============== [Dev] 치트 적용 시작 ==============");

        // 1. [JSON 수정] 코인 대량 추가
        ApplyCoinCheat();

        // 2. [PlayerPrefs 수정] 스테이지 & 스토리 올클리어
        ApplyUnlockCheat();

        Debug.LogWarning("============== [Dev] 치트 적용 완료! ==============");
    }

    private void ApplyCoinCheat()
    {
        SaveDataMirror data = new SaveDataMirror();

        // 기존 파일이 있으면 불러오기 (없으면 0원부터 시작)
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                data = JsonUtility.FromJson<SaveDataMirror>(json);
            }
            catch
            {
                // 파일이 깨졌거나 형식이 다르면 그냥 새로 만듦
                data = new SaveDataMirror();
            }
        }

        // 코인 추가
        data.coin += coinToAdd;

        // 다시 파일로 저장
        string newJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, newJson);

        Debug.Log($"[Dev] 코인 {coinToAdd} 추가됨 (현재: {data.coin})");
    }

    private void ApplyUnlockCheat()
    {
        // 1. 모든 스테이지 클리어 처리
        foreach (string key in allStageKeys)
        {
            PlayerPrefs.SetInt(key, 1);
        }

        // 2. 스토리 해제 (본/해제됨 처리)
        PlayerPrefs.SetInt("Forest_Story12_Shown", 1);
        PlayerPrefs.SetInt("Forest_Story345_Shown", 1);
        PlayerPrefs.SetInt("Sea_Story12_Shown", 1);
        PlayerPrefs.SetInt("Sea_Story345_Shown", 1);

        // (혹시 몰라 예약 변수들은 0으로 꺼둠 -> 이미 봤으니까)
        PlayerPrefs.SetInt("Forest_Story12_Pending", 0);
        PlayerPrefs.SetInt("Forest_Story345_Pending", 0);
        PlayerPrefs.SetInt("Sea_Story12_Pending", 0);
        PlayerPrefs.SetInt("Sea_Story345_Pending", 0);

        // ★ 저장 확정
        PlayerPrefs.Save();

        Debug.Log("[Dev] 모든 스테이지 및 스토리 잠금 해제 완료!");
    }
}