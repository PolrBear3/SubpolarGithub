using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LeftMenuUI
{
    public Image seasonUIBox;
    public Text seasonUIText;

    public Image weatherUIBox;
    public Text weatherUIText;

    public Text currentInGameDayText;
}

public class Default_Menu : MonoBehaviour
{
    public MainGame_Controller controller;

    public LeftMenuUI leftMenu;

    public GameObject nextDayFade;
    public Text moneyText;

    public RectTransform moneyFadeTextTransform;
    public Text moneyFadeText;

    public RectTransform bonusFadeTextTransform;
    public Text bonusFadeText;

    public LeanTweenType moneyTweenType;

    private void Start()
    {
        Money_Text_Update();
    }

    public void Update_UI()
    {
        Current_InGameDay_Text_Update();
        Next_Day_AlphaValueFade_Tween();
        Season_Weather_UI_Update();
    }

    // time system ui
    private void Current_InGameDay_Text_Update()
    {
        leftMenu.currentInGameDayText.text = "Day " + controller.timeSystem.currentInGameDay.ToString();
    }
    private void Next_Day_AlphaValueFade_Tween()
    {
        var rectTranform = nextDayFade.GetComponent<RectTransform>();

        LeanTween.move(rectTranform, new Vector2(0, 0), 0);
        LeanTween.alpha(rectTranform, 1f, 1f);
        LeanTween.alpha(rectTranform, 0f, 1f).setDelay(1f);
        LeanTween.move(rectTranform, new Vector2(0, 640f), 0).setDelay(2f);
    }

    // season and weather ui
    private void Season_Weather_UI_Update()
    {
        leftMenu.seasonUIBox.sprite = controller.timeSystem.currentSeason.seasonUI;
        leftMenu.seasonUIText.text = controller.timeSystem.currentSeason.seasonName;

        leftMenu.weatherUIBox.sprite = controller.eventSystem.currentWeather.weatherUI;
        leftMenu.weatherUIText.text = controller.eventSystem.currentWeather.weatherName;
    }

    // money system ui
    public void Money_Text_Update()
    {
        moneyText.text = "$ " + controller.money.ToString();
    }
    public void Money_Update_Fade_Tween(bool isAdd, int amount)
    {
        // fade 
        LeanTween.move(moneyFadeTextTransform, new Vector2(-4.899982f, -18.7f), 0.5f).setEase(moneyTweenType);
        LeanTween.alphaText(moneyFadeTextTransform, 0.7f, 0.5f);

        // original 
        LeanTween.alphaText(moneyFadeTextTransform, 0f, 0.25f).setDelay(0.5f);
        LeanTween.move(moneyFadeTextTransform, new Vector2(-4.899982f, -8.8f), 0f).setDelay(0.75f);

        if (isAdd)
        {
            moneyFadeText.text = "+$ " + amount.ToString();
        }
        else if (!isAdd)
        {
            moneyFadeText.text = "-$ " + amount.ToString();
        }
    }
    public void Money_withBonus_Update_Fade_Tween(int originalAmount, int bonusAmount)
    {
        // original amount text
        moneyFadeText.text = "+$ " + originalAmount.ToString();
        // fade original amount
        LeanTween.move(moneyFadeTextTransform, new Vector2(-4.899982f, -18.7f), 0.5f).setEase(moneyTweenType);
        LeanTween.alphaText(moneyFadeTextTransform, 0.7f, 0.5f);
        // return
        LeanTween.move(moneyFadeTextTransform, new Vector2(-4.899982f, -8.8f), 0f).setDelay(0.75f);
        LeanTween.alphaText(moneyFadeTextTransform, 0f, 0.25f).setDelay(0.5f);
        
        // bonus amount text
        bonusFadeText.text = "+$ " + bonusAmount.ToString();
        // fade bonus amount
        LeanTween.move(bonusFadeTextTransform, new Vector2(-4.899982f, -18.7f), 0.5f).setEase(moneyTweenType).setDelay(0.6f);
        LeanTween.alphaText(bonusFadeTextTransform, 0.7f, 0.5f).setDelay(0.6f);
        // return
        LeanTween.move(bonusFadeTextTransform, new Vector2(-4.899982f, -8.8f), 0f).setDelay(0.75f).setDelay(1.35f);
        LeanTween.alphaText(bonusFadeTextTransform, 0f, 0.25f).setDelay(1.1f);
    }
}
