using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToNextSlide : MonoBehaviour, IPointerClickHandler
{
    public IntroSlideshow intro;        // 인트로용(없으면 비워도 됨)
    public IntroSlideshow afterQuiz2;   // 퀴즈2 후용(없으면 비워도 됨)
    public IntroSlideshow afterAll345;  // ✅ 퀴즈3/4/5 올클리어 후용

    public void OnPointerClick(PointerEventData eventData)
    {
        // ✅ "지금 재생 중인 슬라이드쇼"만 Next
        if (intro != null && intro.IsShowing)
        {
            intro.NextFromButton();
            return;
        }

        if (afterQuiz2 != null && afterQuiz2.IsShowing)
        {
            afterQuiz2.NextFromButton();
            return;
        }

        if (afterAll345 != null && afterAll345.IsShowing)
        {
            afterAll345.NextFromButton();
            return;
        }
    }
}
