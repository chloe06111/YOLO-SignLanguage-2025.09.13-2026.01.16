using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 를 사용할 때 필요

public class GameManager : MonoBehaviour
{
    public GameObject mainImage;        // 이미지를 담아두는 GameObject
    public Sprite gameOverSpr;          // GAME OVER 이미지
    public Sprite gameClearSpr;         // GAME CLEAR 이미지
    public GameObject panel;            // 패널
    public GameObject restartButton;    // RESTART 버튼
    public GameObject nextButton;       // NEXT 버튼
    Image titleImage;                   // 이미지를 표시하고있는 Image 컴포넌트

    // +++ 시간 제한 추가 +++
    public GameObject timeBar;          // 시간 표시 이미지 
    public GameObject timeText;         // 시간 텍스트
    TimeController timeCnt;             // TimeController

    // +++ 점수 추가 +++
    public GameObject scoreText;        // 점수 텍스트  
    public static int totalScore;       // 점수 총합
    public int stageScore = 0;          // 스테이지 점수

    // +++ 사운드 재생 추가 +++
    public AudioClip meGameOver;        // 게임 오버
    public AudioClip meGameClear;       // 게임 클리어

    // +++ 플레이어 조작 +++
    public GameObject inputUI; // 조작 UI 패널

    // Start is called before the first frame update
    void Start()
    {
        // 이미지 숨기기
        Invoke("InactiveImage", 1.0f);
        // 버튼(패널)을 숨기기
        panel.SetActive(false);

         // +++ 시간 제한 추가 +++
        // TimeController 가져옴
        timeCnt = GetComponent<TimeController>();
        if(timeCnt != null)
        {
            if(timeCnt.gameTime == 0.0f)
            {
                timeBar.SetActive(false);   // 시간 제한이 없으면 숨김
            }
        }

        // +++ 점수 추가 +++
        UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.gameState == "gameclear")
        {
            // 게임 클리어
            mainImage.SetActive(true); // 이미지 표시
            panel.SetActive(true); // 버튼(패널)을 표시
            // RESTART 버튼 무효화 
            Button bt = restartButton.GetComponent<Button>();
            bt.interactable = false;
            mainImage.GetComponent<Image>().sprite = gameClearSpr;
            PlayerController.gameState = "gameend";

              // +++ 시간 제한 추가 +++
            if (timeCnt != null)
            {
                timeCnt.isTimeOver = true; // 시간 카운트 중지

                // +++ 점수 추가 +++
                // 정수에 할당하여 소숫점 버림
                int time = (int)timeCnt.displayTime;
                totalScore += time * 10;        // 남은 시간을 점수에 더한다.
            }
            // +++ 점수 추가 +++
            totalScore += stageScore;
            stageScore = 0;
            UpdateScore();// 점수 갱신

             // +++ 사운드 재생 추가 +++
            // 사운드 재생
            AudioSource soundPlayer = GetComponent<AudioSource>();
            if (soundPlayer != null)
            {
                // BGM 정지
                soundPlayer.Stop(); soundPlayer.PlayOneShot(meGameClear);
            }
            // +++ 플레어이 조작 +++
            inputUI.SetActive(false);   // 조작 UI 숨기기
        }
        else if (PlayerController.gameState == "gameover")
        {
            // 게임 오버
            mainImage.SetActive(true);      // 이미지 표시
            panel.SetActive(true);          // 버튼(패널)을 표시
            // NEXT 버튼 비활성
            Button bt = nextButton.GetComponent<Button>();
            bt.interactable = false;
            mainImage.GetComponent<Image>().sprite = gameOverSpr;
            PlayerController.gameState = "gameend";

             // +++ 시간 제한 추가 +++
            if (timeCnt != null)
            {
                timeCnt.isTimeOver = true; // 시간 카운트 중지
            }

            // +++ 사운드 재생 추가 +++
            // 사운드 재생
            AudioSource soundPlayer = GetComponent<AudioSource>();
            if (soundPlayer != null)
            {
                // BGM 정지
                soundPlayer.Stop();
                soundPlayer.PlayOneShot(meGameOver);
            }
            // +++ 플레어이 조작 +++
            inputUI.SetActive(false);   // 조작 UI 숨기기
        }
        else if (PlayerController.gameState == "playing")
        {
            // 게임 중
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            // PlayerController 가져오기
            PlayerController playerCnt = player.GetComponent<PlayerController>();
            // +++ 시간 제한 추가 +++
            // 시간 갱신
            if(timeCnt != null)
            {
                if (timeCnt.gameTime > 0.0f)
                {
                    // 정수에 할당하여 소수점 이하를 버림
                    int time = (int)timeCnt.displayTime;
                    // 시간 갱신
                    timeText.GetComponent<Text>().text = time.ToString();
                    // 타임 오버
                    if (time == 0)
                    {
                        playerCnt.GameOver();   // 게임 오버
                    }
                }
            }
            // +++ 점수 추가 +++
            if (playerCnt.score != 0)
            {
                stageScore += playerCnt.score;
                playerCnt.score = 0;
                UpdateScore();
            }
        }
    }
    // 이미지 숨기기
    void InactiveImage()
    {
        mainImage.SetActive(false);
    }

     // +++ 점수 추가 +++
    void UpdateScore()
    {
        int score = stageScore + totalScore;
        scoreText.GetComponent<Text>().text = score.ToString();
    }
    
    // +++ 플레이어 조작 +++ 
    // 점프
      public void Jump()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerController playerCnt = player.GetComponent<PlayerController>();
        playerCnt.Jump();
    }
}
