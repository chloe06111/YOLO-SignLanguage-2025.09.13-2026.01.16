using UnityEngine;

public class StoryAutoPlayOnMap : MonoBehaviour
{
    // 퀴즈2 스토리
    public IntroSlideshow slideshow;

    // 퀴즈3/4/5 올클리어 스토리
    public IntroSlideshow slideshowAfterAll345;

    private void Start()
    {
        // =========================
        // ✅ Story12 (퀴즈1+2 완료 후)
        // =========================
        int pending12 = PlayerPrefs.GetInt("Forest_Story12_Pending", 0);
        int q1 = PlayerPrefs.GetInt("Forest_Quiz_1", 0);
        int q2 = PlayerPrefs.GetInt("Forest_Quiz_2", 0);

        if (pending12 == 1 && q1 == 1 && q2 == 1)
        {
            // ✅ pending만 먼저 내려서 중복 방지
            PlayerPrefs.SetInt("Forest_Story12_Pending", 0);
            PlayerPrefs.Save();

            if (slideshow != null)
            {
                // ✅ Begin()이 onceKey를 보고 “이미 봤으면 return” 하는 구조라서
                // ❌ 여기서 Shown을 1로 찍으면 안 됨!
                slideshow.autoPlayOnStart = false;
                slideshow.showOnlyOnce = true;
                slideshow.onceKey = "Forest_Story12_Shown"; // Hide() 될 때 1로 저장됨
                slideshow.Begin();
                return; // ✅ 12를 띄우면 같은 프레임에 345는 안 띄움
            }
            else
            {
                Debug.LogWarning("[StoryAutoPlayOnMap] ❌ slideshow(Story12) Inspector 연결 안 됨");
            }
        }

        // =========================
        // ✅ Story345 (퀴즈3/4/5 올클리어 후)
        // =========================
        int pending345 = PlayerPrefs.GetInt("Forest_Story345_Pending", 0);
        if (pending345 != 1) return;

        int q3 = PlayerPrefs.GetInt("Forest_Quiz_3", 0);
        int q4 = PlayerPrefs.GetInt("Forest_Quiz_4", 0);
        int q5 = PlayerPrefs.GetInt("Forest_Quiz_5", 0);

        // ✅ pending은 먼저 내려서 중복 방지
        PlayerPrefs.SetInt("Forest_Story345_Pending", 0);
        PlayerPrefs.Save();

        // ✅ 3,4,5가 전부 클리어 되었을 때만 재생
        if (!(q3 == 1 && q4 == 1 && q5 == 1)) return;

        if (slideshowAfterAll345 != null)
        {
            // ❌ 여기서도 Shown을 1로 찍으면 Begin이 바로 return해서 안 보임
            slideshowAfterAll345.autoPlayOnStart = false;
            slideshowAfterAll345.showOnlyOnce = true;
            slideshowAfterAll345.onceKey = "Forest_Story345_Shown"; // Hide() 때 1 저장
            slideshowAfterAll345.Begin();
        }
        else
        {
            Debug.LogWarning("[StoryAutoPlayOnMap] ❌ slideshowAfterAll345 Inspector 연결 안 됨");
        }
    }
}
