using UnityEngine;

public class PlayerIdentity : MonoBehaviour
{
    [SerializeField] private string playerId = "default";
    public string PlayerId => playerId;

    public void SetId(string id)
    {
        playerId = id;
    }
}
