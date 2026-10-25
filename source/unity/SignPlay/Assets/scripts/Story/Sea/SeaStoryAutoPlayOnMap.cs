using UnityEngine;

public class SeaStoryAutoPlayOnMap : MonoBehaviour
{
    public IntroSlideshow slideshow; // SeaStoryRoot에 붙은 IntroSlideshow 연결

    // ✅ (추가) 바다 퀴즈1·2 올클리어 스토리
    public IntroSlideshow slideshowAfterQuiz12; // SeaStory12Root에 붙은 IntroSlideshow 연결

    // ✅ (추가) 바다 퀴즈3/4/5 올클리어 스토리
    public IntroSlideshow slideshowAfterAll345; // SeaStory345Root에 붙은 IntroSlideshow 연결

    private void Start()
    {
        int pending = PlayerPrefs.GetInt("Sea_Intro_Pending", 0);
        if (pending == 1)
        {
            // 중복 방지: 먼저 pending 내리기
            PlayerPrefs.SetInt("Sea_Intro_Pending", 0);
            PlayerPrefs.Save();

            if (slideshow == null)
            {
                Debug.LogWarning("[SeaStoryAutoPlayOnMap] slideshow 연결 안 됨");
                return;
            }

            // 1회만 보이게
            slideshow.autoPlayOnStart = false;
            slideshow.showOnlyOnce = true;
            slideshow.onceKey = "Sea_Intro_Shown";

            slideshow.Begin();
            return; // ✅ 인트로를 띄우면 같은 프레임에 다른 스토리는 안 띄움
        }

        // ✅ (추가) 퀴즈1·2 올클리어 후 스토리12 재생
        int pending12 = PlayerPrefs.GetInt("Sea_Story12_Pending", 0);
        if (pending12 == 1)
        {
            int q1 = PlayerPrefs.GetInt("Sea_Quiz_1", 0);
            int q2 = PlayerPrefs.GetInt("Sea_Quiz_2", 0);

            int sh1 = PlayerPrefs.GetInt("Seashore_Quiz_1", 0);
            int sh2 = PlayerPrefs.GetInt("Seashore_Quiz_2", 0);

            // 중복 방지: 먼저 pending 내리기
            PlayerPrefs.SetInt("Sea_Story12_Pending", 0);
            PlayerPrefs.Save();

            // ✅ 둘 중 하나 체계로 저장돼도 통과
            bool done1 = (q1 == 1 || sh1 == 1);
            bool done2 = (q2 == 1 || sh2 == 1);

            if (done1 && done2)
            {
                if (slideshowAfterQuiz12 == null)
                {
                    Debug.LogWarning("[SeaStoryAutoPlayOnMap] slideshowAfterQuiz12 연결 안 됨");
                    return;
                }

                // 1회만 보이게
                slideshowAfterQuiz12.autoPlayOnStart = false;
                slideshowAfterQuiz12.showOnlyOnce = true;
                slideshowAfterQuiz12.onceKey = "Sea_Story12_Shown";

                slideshowAfterQuiz12.Begin();
                return; // ✅ 12를 띄우면 같은 프레임에 345는 안 띄움
            }
        }

        // ✅ (추가) 퀴즈3·4·5 올클리어 후 스토리345 재생
        int pending345 = PlayerPrefs.GetInt("Sea_Story345_Pending", 0);
        if (pending345 != 1) return;

        int q3 = PlayerPrefs.GetInt("Sea_Quiz_3", 0);
        int q4 = PlayerPrefs.GetInt("Sea_Quiz_4", 0);
        int q5 = PlayerPrefs.GetInt("Sea_Quiz_5", 0);

        int sh3 = PlayerPrefs.GetInt("Seashore_Quiz_3", 0);
        int sh4 = PlayerPrefs.GetInt("Seashore_Quiz_4", 0);
        int sh5 = PlayerPrefs.GetInt("Seashore_Quiz_5", 0);

        // ✅ pending은 먼저 내려서 중복 방지
        PlayerPrefs.SetInt("Sea_Story345_Pending", 0);
        PlayerPrefs.Save();

        bool done3 = (q3 == 1 || sh3 == 1);
        bool done4 = (q4 == 1 || sh4 == 1);
        bool done5 = (q5 == 1 || sh5 == 1);

        if (!(done3 && done4 && done5)) return;

        if (slideshowAfterAll345 == null)
        {
            Debug.LogWarning("[SeaStoryAutoPlayOnMap] slideshowAfterAll345 연결 안 됨");
            return;
        }

        slideshowAfterAll345.autoPlayOnStart = false;
        slideshowAfterAll345.showOnlyOnce = true;
        slideshowAfterAll345.onceKey = "Sea_Story345_Shown";

        slideshowAfterAll345.Begin();
    }
}
