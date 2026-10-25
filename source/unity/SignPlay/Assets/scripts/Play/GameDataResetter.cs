using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement; // 씬 재시작을 위해 추가

public class GameDataResetter : MonoBehaviour
{// GameDataResetter.cs

    public void OnClickResetData()
    {
        Debug.LogWarning("============== [버튼 입력] 데이터 초기화 시작 ==============");

        // 1. JSON 파일 삭제
        string savePath = Path.Combine(Application.persistentDataPath, "savefile_simple.json");
        if (File.Exists(savePath)) File.Delete(savePath);

        // 2. PlayerPrefs 삭제
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // 확실하게 저장

        // 3. [핵심] 메모리 데이터 초기화 호출
        if (QuizGameManager.Instance != null)
        {
            QuizGameManager.Instance.ClearMemoryData(); // <--- 이거 추가!
        }

        // 4. 씬 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}