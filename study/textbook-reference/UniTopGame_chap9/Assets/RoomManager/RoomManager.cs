using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    // static 변수
    public static int doorNumber = 0;   //문 번

    // Start is called before the first frame update
    void Start()
    {
        //플레이어 캐릭터 위치
        //출입구를 배열로 얻기
        GameObject[] enters = GameObject.FindGameObjectsWithTag("Exit");
        for (int i = 0; i < enters.Length; i++)
        {
            GameObject doorObj = enters[i]; //배열에서 꺼내기
            Exit exit = doorObj.GetComponent<Exit>();   //Exit 클래스 변수
            if (doorNumber == exit.doorNumber)
            {
                //==== 같은 문  번호 ====
                //플레이어 캐릭터를 출입구로 이동
                float x = doorObj.transform.position.x;
                float y = doorObj.transform.position.y;
                if (exit.direction == ExitDirection.up)
                {
                    y += 1;
                }
                else if (exit.direction == ExitDirection.right)
                {
                    x += 1;
                }
                else if (exit.direction == ExitDirection.down)
                {
                    y -= 1;
                }
                else if (exit.direction == ExitDirection.left)
                {
                    x -= 1;
                }
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position = new Vector3(x, y);
                break;  //반복문 빠나오기
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //씬 이동
    public static void ChangeScene(string scnename, int doornum)
    {
        doorNumber = doornum;   //문 번호를 static 변수에 저장

        //string nowScene = PlayerPrefs.GetString("LastScene");
        //if (nowScene != "")
        //{
        //    SaveDataManager.saveArrangeData(nowScene);      //配置データを保存
        //}
        //PlayerPrefs.SetString("LastScene", scnename);   //シーン名を保存
        //PlayerPrefs.SetInt("LastDoor", doornum);        //ドア番号保存
        //ItemKeeper.SaveItem();                          //アイテムを保存

        SceneManager.LoadScene(scnename);   //シーン移動
    }
}
