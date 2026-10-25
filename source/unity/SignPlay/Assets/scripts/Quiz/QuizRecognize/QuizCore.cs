using UnityEngine;
using TMPro;
using System.Text;

public class QuizCore : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI wordText;   // 말풍선 안 단어 (color_Text)
    public QuizUI quizUI;              // Quiz# 1/5 표시 담당 (QuizUI 스크립트)

    private string currentWord = "";
    private int currentLetterIndex = 0;   // 몇 글자까지 맞췄는지

    private void Start()
    {
        // 1. 퀴즈 씬에 진입했으니, 매니저의 상태를 초기화합니다!
        // ⚠️ 여기서 ResetQuizSession()을 호출하면
        // 씬 이동 시 hintUsedCount가 0으로 초기화되어
        // 엔딩 씬에서 항상 별 3개가 뜨는 문제가 발생함
        // ❌ 제거함

        // 2. 그 다음, 0번 문제부터 로드합니다.
        LoadQuestion();
    }

    // ============= 1) 현재 퀴즈 단어 불러오기 =============
    public void LoadQuestion()
    {
        var gm = QuizGameManager.Instance;
        if (gm == null || gm.TotalQuizCount == 0) return;

        int idx = gm.CurrentQuizIndex;
        if (idx >= gm.TotalQuizCount) return;

        currentWord = gm.quizWords[idx].ToLower();   // 예: "green"
        currentLetterIndex = 0;

        UpdateWordDisplay();

        if (quizUI != null)
        {
            // Quiz# 1/5, 2/5 이런 거 업데이트
            quizUI.UpdateQuizNumber(idx + 1);
        }
    }

    // 단어를 표시하면서 이미 맞춘 글자는 회색으로
    private void UpdateWordDisplay()
    {
        if (string.IsNullOrEmpty(currentWord) || wordText == null) return;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < currentWord.Length; i++)
        {
            char c = currentWord[i];

            if (i < currentLetterIndex)
            {
                // 이미 인식 성공한 글자 → 회색 하이라이트
                sb.Append("<color=#808080>");
                sb.Append(c);
                sb.Append("</color>");
            }
            else
            {
                sb.Append(c);
            }
        }

        wordText.text = sb.ToString();
    }

    // ============= 2) Python에서 받은 글자 체크 =============
    public void CheckAnswer(string answer)
    {
        if (string.IsNullOrEmpty(answer)) return;
        if (string.IsNullOrEmpty(currentWord)) return;

        answer = answer.Trim().ToLower();
        if (answer == "unknown") return;   // 인식 불확실한 경우 무시

        char receivedChar = answer[0];

        if (currentLetterIndex >= currentWord.Length) return;

        char expectedChar = currentWord[currentLetterIndex];

        Debug.Log($"[QuizCore] Expected: {expectedChar}, Received: {receivedChar}");

        if (receivedChar == expectedChar)
        {
            // 한 글자 정답!
            currentLetterIndex++;
            UpdateWordDisplay();

            // 단어 전체 끝났는지 확인
            if (currentLetterIndex >= currentWord.Length)
            {
                Debug.Log($"[QuizCore] Word '{currentWord}' completed!");
                GoToNextQuestion();
            }
        }
        else
        {
            Debug.Log($"[QuizCore] Wrong letter. Expected {expectedChar}, got {receivedChar}");
        }
    }

    // ============= 3) 다음 문제로 이동 =============
    public void GoToNextQuestion()
    {
        var gm = QuizGameManager.Instance;
        if (gm == null) return;

        gm.NextQuiz();

        // 모든 문제 끝났으면 결과로
        if (gm.CurrentQuizIndex >= gm.TotalQuizCount)
        {
            Debug.Log("[QuizCore] All quiz completed. Go to result scene here.");
            // 나중에 SceneJump 써서 결과 씬으로 이동
            return;
        }

        // 다음 단어 로드
        LoadQuestion();
    }
}
