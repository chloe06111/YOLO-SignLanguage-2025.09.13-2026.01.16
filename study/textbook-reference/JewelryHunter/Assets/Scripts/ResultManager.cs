using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject backButton;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.GetComponent<Text>().text = GameManager.totalScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            ChangeScene cs = backButton.GetComponent<ChangeScene>();
            if (cs != null)
            {
                cs.Load();
            }
        }
    }
}
