using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int arrangeId = 0;       //식별에 사용하기 위한 값

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //열쇠를 가지고있으면
            if (ItemKeeper.hasKeys > 0)
            {
                ItemKeeper.hasKeys--;       //열쇠를 하나 감소
                Destroy(this.gameObject);   //문 열기
            }
        }
    }
}
