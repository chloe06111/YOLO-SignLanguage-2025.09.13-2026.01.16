using UnityEngine;

public class LaunchSkinResetter : MonoBehaviour
{
    [Header("Prefs Key")]
    [SerializeField] private string selectedPlayerKey = "SelectedPlayer";

    [Header("Default Skin Key")]
    [SerializeField] private string defaultSkin = "default";

    [Header("Run Once Per Launch")]
    [SerializeField] private bool runOnlyOnce = true;

    private const string RanKey = "_LaunchSkinReset_Ran";

    private void Awake()
    {
        if (runOnlyOnce && PlayerPrefs.GetInt(RanKey, 0) == 1)
            return;

        PlayerPrefs.SetString(selectedPlayerKey, defaultSkin);
        PlayerPrefs.SetInt(RanKey, 1);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        // 다음 실행 때 다시 리셋되게 하려면 Quit 때 플래그를 지움
        if (runOnlyOnce)
        {
            PlayerPrefs.DeleteKey(RanKey);
            PlayerPrefs.Save();
        }
    }
}
