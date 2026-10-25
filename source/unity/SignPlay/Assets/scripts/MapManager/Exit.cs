﻿using UnityEngine;
using UnityEngine.SceneManagement;

public enum ExitDirection
{
    right,
    left,
    down,
    up,
}

public class Exit : MonoBehaviour
{
    public string sceneName = "";
    public int doorNumber = 0;
    public ExitDirection direction = ExitDirection.down;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.enabled == false) return;
        if (!collision.CompareTag("Player")) return;

        //// ✅ 메인맵 -> 바다맵으로 "들어가기 직전" 위치를 저장 (돌아올 때 그 자리로 스폰)
        //if (sceneName == "SeashoreMapScene" && QuizGameManager.Instance != null)
        //{
        //    QuizGameManager.Instance.SetLastPosition(collision.transform.position);
        //}

        // ✅ 메인맵 → 바다맵(SeashoreMapScene) 이동 직전에 스토리 재생 예약
        if (sceneName == "SeashoreMapScene" && PlayerPrefs.GetInt("Sea_Intro_Shown", 0) == 0)
        {
            PlayerPrefs.SetInt("Sea_Intro_Pending", 1);
            PlayerPrefs.Save();
        }

        RoomManager.ChangeScene(sceneName, doorNumber);
    }
}
