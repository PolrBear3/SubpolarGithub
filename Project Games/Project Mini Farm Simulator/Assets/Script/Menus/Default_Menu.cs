using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Default_Menu_UI
{
    public Image seasonUIBox;
    public Text seasonUIText;

    public Image weatherUIBox;
    public Text weatherUIText;

    public Text currentInGameDayText;

    public Text remainingTimeText;
    public GameObject[] nextDayButtons;
    public Button nextDayButton;
}

public class Default_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    public Button[] allAvailableButtons;

    public Default_Menu_UI menuUI;

    public GameObject nextDayFade;

    public RectTransform moneyTextRT;
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

    public void Button_Shield(bool activate)
    {
        for (int i = 0; i < allAvailableButtons.Length; i++)
        {
            if (activate) { allAvailableButtons[i].enabled = false; }
            else if (!activate) { allAvailableButtons[i].enabled = true; }
        }
    }
    
    // ui ingame update
    public void Update_UI()
    {
        Current_InGameDay_Text_Update();
        Next_Day_AlphaValueFade_Tween();
        Season_Weather_UI_Update();
    }
    public void Activate_NextDay_Button(bool activation)
    {
        if (activation)
        {
            menuUI.nextDayButtons[0].SetActive(false);
            menuUI.nextDayButtons[1].SetActive(true);
        }
        else
        {
            menuUI.nextDayButtons[0].SetActive(true);
            menuUI.nextDayButtons[1].SetActive(false);
        }
    }

    // time system ui
    private void Current_InGameDay_Text_Update()
    {
        menuUI.currentInGameDayText.text = "Day " + controller.timeSystem.currentInGameDay.ToString();
    }
    private void Next_Day_AlphaValueFade_Tween()
    {
        var rectTransform = nextDayFade.GetComponent<RectTransform>();
        var image = nextDayFade.GetComponent<Image>();

        image.sprite = controller.eventSystem.currentWeather.fadeBackgroundUI;
        LeanTween.move(rectTransform, new Vector2(0, 0), 0);
        LeanTween.alpha(rectTransform, 1f, 1f);
        LeanTween.alpha(rectTransform, 0f, 1f).setDelay(1f);
        LeanTween.move(rectTransform, new Vector2(0, 640f), 0).setDelay(2f);
    }

    // season and weather ui
    private void Season_Weather_UI_Update()
    {
        menuUI.seasonUIBox.sprite = controller.timeSystem.currentSeason.seasonUI;
        menuUI.seasonUIText.text = controller.timeSystem.currentSeason.seasonName;

        menuUI.weatherUIBox.sprite = controller.eventSystem.currentWeather.weatherUI;
        menuUI.weatherUIText.text = controller.eventSystem.currentWeather.weatherName;
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
            LeanTween.textColor(moneyFadeTextTransform, Color.red, 0);
            LeanTween.textColor(moneyFadeTextTransform, new Color32(30, 63, 45, 255), 0.2f).setDelay(0.31f);
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
        // money text double bounce
        LeanTween.move(moneyTextRT, new(-4.9f, -2f), 0.2f).setEase(moneyTweenType).setDelay(0.6f);
        LeanTween.move(moneyTextRT, new(-4.9f, -4f), 0.2f).setEase(moneyTweenType).setDelay(0.9f);
        // return
        LeanTween.move(bonusFadeTextTransform, new Vector2(-4.899982f, -8.8f), 0f).setDelay(0.75f).setDelay(1.35f);
        LeanTween.alphaText(bonusFadeTextTransform, 0f, 0.25f).setDelay(1.1f);
    }

    public void NotEnoughMoney_Blink_Tween()
    {
        // color
        LeanTween.textColor(moneyTextRT, Color.red, 0.2f);
        // color return
        LeanTween.textColor(moneyTextRT, new Color32(30, 63, 45, 255), 0.2f).setDelay(0.3f);

        // bounce
        LeanTween.move(moneyTextRT, new(-4.9f, -3f), 0.2f).setEase(moneyTweenType);
        // bounce return
        LeanTween.move(moneyTextRT, new(-4.9f, -4f), 0.2f).setEase(moneyTweenType).setDelay(0.3f);
    }
    public void SubtractMoney_RedBlink()
    {
        // color
        LeanTween.textColor(moneyTextRT, Color.red, 0.2f);
        // color return
        LeanTween.textColor(moneyTextRT, new Color32(30, 63, 45, 255), 0.2f).setDelay(0.3f);

        // bounce
        LeanTween.move(moneyTextRT, new(-4.9f, -2f), 0.2f).setEase(moneyTweenType);
        // bounce return
        LeanTween.move(moneyTextRT, new(-4.9f, -4f), 0.2f).setEase(moneyTweenType).setDelay(0.3f);
    }
    public void AddMoney_Blink()
    {
        // bounce
        LeanTween.move(moneyTextRT, new(-4.9f, -2f), 0.2f).setEase(moneyTweenType);
        // bounce return
        LeanTween.move(moneyTextRT, new(-4.9f, -4f), 0.2f).setEase(moneyTweenType).setDelay(0.3f);
    }
}
