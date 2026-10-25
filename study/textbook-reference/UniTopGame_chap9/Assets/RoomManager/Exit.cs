using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//출입구 위치
public enum ExitDirection
{
    right,  //오른쪽
    left,   //왼쪽
    down,   //아래쪽
    up,     //위쪽
}

public class Exit : MonoBehaviour
{
    public string sceneName = "";   //이동할 씬 이름
    public int doorNumber = 0;      //문 번호
    public ExitDirection direction = ExitDirection.down;//문의 위치

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            RoomManager.ChangeScene(sceneName, doorNumber);
        }
    }
}

