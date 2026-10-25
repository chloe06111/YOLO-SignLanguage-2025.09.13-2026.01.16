using UnityEngine;

[CreateAssetMenu(menuName = "Costume/Database")]
public class CostumeDatabase : ScriptableObject
{
    [System.Serializable]
    public class CostumeEntry
    {
        public string id;
        public AnimatorOverrideController overrideController;
    }

    public CostumeEntry[] costumes;

    public AnimatorOverrideController Get(string id)
    {
        foreach (var c in costumes)
        {
            if (c.id == id)
                return c.overrideController;
        }
        return null;
    }
}
