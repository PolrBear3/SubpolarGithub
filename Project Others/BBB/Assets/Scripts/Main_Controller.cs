using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Main_Controller : MonoBehaviour
{
    [SerializeField] private Sprite[] letters;
    [SerializeField] private Image letterBox;
    [SerializeField] private GameObject timeBox;

    [SerializeField] private TMP_Text timeText;

    [SerializeField] private float setTime;
    private float mytime;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Get_Letter();
        }

        Count_Timer();
        Show_Letter();
    }

    private void Count_Timer()
    {
        if (mytime > 0.6)
        {
            mytime -= Time.deltaTime;
            Show_Time();
        }
    }

    private void Show_Time()
    {
        timeText.text = mytime.ToString("f0");
    }

    private void Get_Letter()
    {
        int randNum = Random.Range(0, letters.Length);
        letterBox.sprite = letters[randNum];

        mytime = setTime + 0.5f;
    }

    private void Show_Letter()
    {
        if (mytime <= 0.6)
        {
            timeBox.SetActive(false);
            Show_Time();
        }
        else
        {
            timeBox.SetActive(true);
        }
    }
}
