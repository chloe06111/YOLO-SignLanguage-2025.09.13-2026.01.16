using UnityEngine;

public class PathLockController : MonoBehaviour
{
    [Header("해금 조건 (이 두 개를 깨야 열림)")]
    public string conditionKey1 = "Forest_Quiz_1";
    public string conditionKey2 = "Forest_Quiz_2";

    [Header("색상 설정")]
    public Color lockedColor = Color.gray; 

    public Color unlockedColor = new Color32(235, 151, 43, 255);

    void Start()
    {
        Invoke("CheckAndSetColor", 0.1f);
    }

    void CheckAndSetColor()
    {
        bool isClear1 = PlayerPrefs.GetInt(conditionKey1, 0) == 1;
        bool isClear2 = PlayerPrefs.GetInt(conditionKey2, 0) == 1;

        Debug.Log($"[PathLock] {gameObject.name} 상태: 퀴즈1({isClear1}), 퀴즈2({isClear2})");

        if (isClear1 && isClear2)
        {
            ApplyColor(unlockedColor);
        }
        else
        {
            ApplyColor(lockedColor);
        }
    }

    void ApplyColor(Color targetColor)
    {
        LineRenderer line = GetComponent<LineRenderer>();
        if (line != null)
        {
            line.startColor = targetColor;
            line.endColor = targetColor;
        }

        SpriteRenderer[] dots = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer dot in dots)
        {
            dot.color = targetColor;
        }
    }
}