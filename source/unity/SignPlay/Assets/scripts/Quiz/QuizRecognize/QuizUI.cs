using UnityEngine;
using TMPro;

public class QuizUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI quizNumberText;   // "Quiz# 1/5" 표시용

    public void UpdateQuizNumber(int num)
    {
        var gm = QuizGameManager.Instance;
        if (gm == null || quizNumberText == null) return;

        // 예: "Quiz# 1/5" 형태로 보여주고 싶으면 아래처럼
        quizNumberText.text = "Quiz# " + gm.GetProgressText(); //일단 스타트는 Quiz# 1/5 처럼 보일것. 
    }

    public void ShowResultUI()
    {
        // 나중에 결과 씬에서 쓸 예정
    }
}
