using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //이동속도
    public float speed = 3.0f;
    //애니매이션 이름
    public string upAnime = "PlayerUp"; //위
    public string downAnime = "PlayerDown"; //아래
    public string rightAnime = "PlayerRight"; //오른족
    public string leftAnime = "PlayerLeft"; //왼쪽
    public string deadAnime = "PlayerDead"; //사망
    //현재 애니메이션
    string nowAnimation = "";
    //이전 애니메이션
    string oldAnimation = "";

    float axisH;
    float axisV;
    public float angleZ = -90.0f; //회전각

    Rigidbody2D rbody;
    bool isMoving = false; 

    //start is called before the first frame update
    void Start()
    {
        //Rigidbody2D 가져오기
        rbody = GetComponent<Rigidbody2D>();
        //애니메이션
        oldAnimation = downAnime;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving == false)
        {
            axisH = Input.GetAxisRaw("Horizontal"); // 좌우 키 입력
            axisV = Input.GetAxisRaw("Vertical"); //상하 키 입력

        }
        //키 입력으로 이동 각도 구하기
        Vector2 fromPt = transform.position;
        Vector2 toPt = new Vector2(fromPt.x + axisH, fromPt.y + axisV);
        angleZ = GetAngle(fromPt, toPt);
        //이동 각도에서 방향과 애니메이션 변경
        if (angleZ >= -45 && angleZ < 45)
        {
            //오른쪽
            nowAnimation = rightAnime;
        }
        else if (angleZ >= 45 && angleZ <= 135)
        {
            //위쪽
            nowAnimation = upAnime;
        }
        else if (angleZ >= -135 && angleZ <= -45)
        {
            //아래쪽
            nowAnimation = downAnime;
        }
        else
        {
            //왼쪽
            nowAnimation = leftAnime;
        }
        //애니메이션 변경하기
        if (nowAnimation != oldAnimation)
        {
            oldAnimation = nowAnimation;
            GetComponent<Animator>().Play(nowAnimation);
        }
    }
    void FixedUpdate()
    {
        //이동속도 변경하기
        rbody.velocity = new Vector2(axisH, axisV) * speed;
    }
    public void SetAxis(float h, float v)
    {
        axisH = h;
        axisV = v;
        if (axisH == 0 && axisV == 0)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }
    //p1에서 p2까지의 각도를 계산
    float GetAngle(Vector2 p1, Vector2 p2)
    {
        float angle;
        if (axisH != 0 || axisV != 0)
        {
            //이동 중이면 각도를 변경
            //p1 과 p2차이 구하기 (원점을 0으로 하기 위해)
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;
            //아크 탄젠트 함수로 각도(라디안) 구하기
            float rad = Mathf.Atan2(dy, dx);
            //라디안 각으로 변환하여 반환
            angle = rad * Mathf.Rad2Deg;
        }
        else
        {
            //정지 중이면 이전 각도를 유지
            angle = angleZ;
        }
        return angle; 
    }
}

