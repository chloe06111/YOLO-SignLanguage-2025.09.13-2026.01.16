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

    // Start is called before the first frame update
    void Start()
    {
        // 이미지 숨기기
        Invoke("InactiveImage", 1.0f);
        // 버튼(패널)을 숨기기
        panel.SetActive(false);
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
        }
        else if (PlayerController.gameState == "playing")
        {
            // 게임 중

        }
    }
    // 이미지 숨기기
    void InactiveImage()
    {
        mainImage.SetActive(false);
    }
}
