using UnityEngine;
using TMPro;

public class MapScoreUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText; // 진행도 텍스트
    public TextMeshProUGUI coinText;  // 코인 텍스트

    [Header("Settings")]
    public int maxScore = 30; // ✅ 전체 만점 (기본값 30)

    private void Start()
    {
        UpdateScoreUI();
    }

    private void OnEnable()
    {
        UpdateScoreUI();
    }

    public void UpdateScoreUI() // ★ private -> public 으로 변경
    {
        if (QuizGameManager.Instance == null) return;

        // 1. 진행도 표시 (예: "5 / 30")
        if (scoreText != null)
        {
            // ✅ 현재 점수 / 만점 형태로 표시
            scoreText.text = $"{QuizGameManager.Instance.score} / {maxScore}";
        }

        // 2. 코인 표시
        if (coinText != null)
        {
            coinText.text = QuizGameManager.Instance.coin.ToString();
        }
    }
}