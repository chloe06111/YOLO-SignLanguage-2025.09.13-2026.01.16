using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f;          // 이동 속도
    public string direction = "left";   // 방향 right or left 
    public float range = 0.0f;          // 움직이는 범위
    Vector3 defPos;                     // 시작 위치

    // Start is called before the first frame update
    void Start()
    {
        if (direction == "right")
        {
            transform.localScale = new Vector2(-1, 1);// 방향 변경
        }
        // 시작 위
        defPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (range > 0.0f)
        {
            if (transform.position.x < defPos.x - (range / 2))
            {
                direction = "right";
                transform.localScale = new Vector2(-1, 1);// 방향 변경
                if (transform.position.x > defPos.x + (range / 2)){
                    direction = "left";
                    transform.localScale = new Vector2(1, 1);// 방향 변경
                }
            }
            
            
        }
    }

    void FixedUpdate()
    {
        // 속도 갱신
        // Rigidbody2D 가져오기
        Rigidbody2D rbody = GetComponent<Rigidbody2D>();
        if (direction == "right")
        {
            rbody.velocity = new Vector2(speed, rbody.velocity.y);
        }
        else
        {
            rbody.velocity = new Vector2(-speed, rbody.velocity.y);
        }
    }

    // 접촉
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (direction == "right")
        {
            direction = "left";
            transform.localScale = new Vector2(1, 1); // 방향 변경
        }
        else
        {
            direction = "right";
            transform.localScale = new Vector2(-1, 1); // 방향 변경
        }
    }
}
