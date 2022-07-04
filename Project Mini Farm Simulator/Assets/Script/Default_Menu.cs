using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Default_Menu : MonoBehaviour
{
    public MainGame_Controller controller;

    public GameObject nextDayFade;
    public Text currentInGameDayText, moneyText;

    public RectTransform addMoneyFadeTextBox;
    public Text addMoneyFadeText;
    public LeanTweenType addMoneyTweenType;

    private void Start()
    {
        Money_Text_Update();
    }

    public void Update_UI()
    {
        Current_InGameDay_Text_Update();
        Next_Day_AlphaValueFade_Tween();
    }

    // time system ui
    private void Current_InGameDay_Text_Update()
    {
        currentInGameDayText.text = "Day " + controller.timeSystem.currentInGameDay.ToString();
    }
    private void Next_Day_AlphaValueFade_Tween()
    {
        var rectTranform = nextDayFade.GetComponent<RectTransform>();

        LeanTween.move(rectTranform, new Vector2(0, 0), 0);
        LeanTween.alpha(rectTranform, 1f, 1f);
        LeanTween.alpha(rectTranform, 0f, 1f).setDelay(1f);
        LeanTween.move(rectTranform, new Vector2(0, 640f), 0).setDelay(2f);
    }

    // money system ui
    public void Money_Text_Update()
    {
        moneyText.text = "$ " + controller.money.ToString();
    }
    public void Money_Update_Fade_Tween(bool isAdd, int amount)
    {
        // fade 
        LeanTween.move(addMoneyFadeTextBox, new Vector2(-4.899982f, -18.7f), 0.5f).setEase(addMoneyTweenType);
        LeanTween.alphaText(addMoneyFadeTextBox, 0.7f, 0.5f);

        // original 
        LeanTween.alphaText(addMoneyFadeTextBox, 0f, 0.25f).setDelay(0.5f);
        LeanTween.move(addMoneyFadeTextBox, new Vector2(-4.899982f, -8.8f), 0f).setDelay(0.75f);

        if (isAdd)
        {
            addMoneyFadeText.text = "+$ " + amount.ToString();
        }
        else if (!isAdd)
        {
            addMoneyFadeText.text = "-$ " + amount.ToString();
        }
    }
}
