using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Player (Fixed)")]
    [SerializeField] private bool preventDuplicatePlayer = true; // 씬에 Player가 이미 있으면 새로 안 만들기
    [SerializeField] private bool forcePlayerTag = true;         // Player Tag를 "Player"로 강제

    [Header("Camera")]
    [SerializeField] private CameraManager cameraManager;        // 비워두면 자동으로 찾음

    // 스킨(Animator Override) DB
    [System.Serializable]
    public class SkinEntry
    {
        public string key; // "default", "sunglasses", "rainbow"
        public AnimatorOverrideController overrideController;
    }

    [Header("Skin Override DB (Drag & Drop here)")]
    [SerializeField] private SkinEntry[] skinDB;

    private const string SelectedPlayerKey = "SelectedPlayer";

    private void Start()
    {
        // 1) 플레이어 찾기(고정 플레이어 전제)
        GameObject player = null;
        try { player = GameObject.FindGameObjectWithTag("Player"); }
        catch { player = null; }

        if (player == null)
        {
            // 이 버전은 "프리팹 생성"이 목적이 아니라서,
            // 씬에 Player가 없으면 그냥 경고만 (게임 씬에는 Player가 있어야 함)
            Debug.LogWarning("[PlayerSpawnManager] 씬에 Tag=Player 오브젝트가 없습니다. (고정 플레이어 방식에서는 Player가 씬에 있어야 해요)");
            return;
        }

        if (forcePlayerTag)
        {
            try { player.tag = "Player"; }
            catch { Debug.LogWarning("[PlayerSpawnManager] Tag 'Player' 설정 불가. (Tags에 Player 추가 필요)"); }
        }

        // 2) 카메라 연결
        LinkCamera(player.transform);

        // 3) 저장된 스킨 적용
        ApplySelectedPlayerNow();
    }

    //  상점에서 APPLY 눌렀을 때 / 씬 시작할 때 호출하면 현재 Player에 스킨 즉시 적용
    public void ApplySelectedPlayerNow()
    {
        string key = PlayerPrefs.GetString(SelectedPlayerKey, "default");

        GameObject player = null;
        try { player = GameObject.FindGameObjectWithTag("Player"); }
        catch { player = null; }

        if (player == null)
        {
            // 상점 씬처럼 플레이어 없으면 그냥 스킵
            Debug.Log("[PlayerSpawnManager] Player가 없어서 스킨 적용을 건너뜁니다. (상점 씬이면 정상)");
            return;
        }

        Animator anim = player.GetComponentInChildren<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("[PlayerSpawnManager] Player에서 Animator를 찾지 못했어요.");
            return;
        }

        AnimatorOverrideController target = FindSkin(key);
        if (target == null)
        {
            Debug.LogWarning($"[PlayerSpawnManager] '{key}' 스킨이 DB에 없어서 default로 대체합니다.");
            target = FindSkin("default");
        }

        if (target == null)
        {
            Debug.LogError("[PlayerSpawnManager] default 스킨(overrideController)이 DB에 없어요. Inspector에서 연결하세요.");
            return;
        }

        anim.runtimeAnimatorController = target;
        Debug.Log($"[PlayerSpawnManager] Skin applied: {key}");
    }

    private AnimatorOverrideController FindSkin(string key)
    {
        if (skinDB == null) return null;

        for (int i = 0; i < skinDB.Length; i++)
        {
            if (skinDB[i] != null && skinDB[i].key == key)
                return skinDB[i].overrideController;
        }
        return null;
    }

    private void LinkCamera(Transform playerTransform)
    {
        if (cameraManager == null)
            cameraManager = FindObjectOfType<CameraManager>();

        if (cameraManager != null)
        {
            cameraManager.SetTarget(playerTransform);
        }
        else
        {
            Debug.LogWarning("[PlayerSpawnManager] CameraManager를 씬에서 찾지 못했어요. 카메라가 따라오지 않을 수 있어요.");
        }
    }
}
