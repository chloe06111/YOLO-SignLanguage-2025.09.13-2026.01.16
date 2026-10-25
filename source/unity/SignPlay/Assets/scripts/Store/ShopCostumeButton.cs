using UnityEngine;

public class ShopCostumeButton : MonoBehaviour
{
    [Header("Costume")]
    [SerializeField] private string costumeId = "rainbow";

    private const string EquippedCostumeKey = "EquippedCostume";

    /// <summary>
    /// 상점 버튼 OnClick에 연결
    /// - Player가 없는 씬이므로 즉시 적용
    /// - 저장만 하고, 다음에 맵 씬에서 Player가 생성될 때 자동 적용
    /// </summary>
    public void OnClickEquip()
    {
        PlayerPrefs.SetString(EquippedCostumeKey, costumeId);
        PlayerPrefs.Save();

        Debug.Log($"[Shop] Costume saved: {costumeId}");
    }
}
