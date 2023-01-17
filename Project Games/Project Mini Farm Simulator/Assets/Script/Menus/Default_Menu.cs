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

[System.Serializable]
public class Default_Menu_UI_Previous_Data
{
    public RectTransform seasonBox;
    public RectTransform seasonText;

    public RectTransform dayBox;
    public RectTransform dayText;

    public RectTransform weatherBox;
    public RectTransform weatherText;
}

public class Default_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    public Button[] allAvailableButtons;

    public Default_Menu_UI menuUI;

    [SerializeField] private Default_Menu_UI_Previous_Data previousFadeData;
    [SerializeField] private RectTransform nextDayFade;
    [SerializeField] private RectTransform fadeInGameDayText;

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
        Season_UI_Update();
        Weather_UI_Update();
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
    public void Current_InGameDay_Text_Update()
    {
        menuUI.currentInGameDayText.text = "Day " + controller.timeSystem.currentInGameDay.ToString();
    }
    public void Next_Day_FadeIn_Previous_Data()
    {
        var seasonBox = previousFadeData.seasonBox.GetComponent<Image>();
        seasonBox.sprite = menuUI.seasonUIBox.sprite;
        var seasonText = previousFadeData.seasonText.GetComponent<Text>();
        seasonText.text = menuUI.seasonUIText.text;

        var dayText = previousFadeData.dayText.GetComponent<Text>();
        dayText.text = menuUI.currentInGameDayText.text;

        var weatherBox = previousFadeData.weatherBox.GetComponent<Image>();
        weatherBox.sprite = menuUI.weatherUIBox.sprite;
        var weatherText = previousFadeData.weatherText.GetComponent<Text>();
        weatherText.text = menuUI.weatherUIText.text;

        // fade in default menu data
        LeanTween.alpha(previousFadeData.seasonBox, 1f, 0f);
        LeanTween.alpha(previousFadeData.seasonText, 1f, 0f);
        LeanTween.alpha(previousFadeData.dayBox, 1f, 0f);
        LeanTween.alpha(previousFadeData.dayText, 1f, 0f);
        LeanTween.alpha(previousFadeData.weatherBox, 1f, 0f);
        LeanTween.alpha(previousFadeData.weatherText, 1f, 0f);

        // fade out default menu data
        LeanTween.alpha(previousFadeData.seasonBox, 0f, 0f).setDelay(3f);
        LeanTween.alpha(previousFadeData.seasonText, 0f, 0f).setDelay(3f);
        LeanTween.alpha(previousFadeData.dayBox, 0f, 0f).setDelay(3f);
        LeanTween.alpha(previousFadeData.dayText, 0f, 0f).setDelay(3f);
        LeanTween.alpha(previousFadeData.weatherBox, 0f, 0f).setDelay(3f);
        LeanTween.alpha(previousFadeData.weatherText, 0f, 0f).setDelay(3f);
    }
    public void Next_Day_Animation()
    {
        var rectTransform = nextDayFade.GetComponent<RectTransform>();
        var text = fadeInGameDayText.GetComponent<Text>();
        var image = nextDayFade.GetComponent<Image>();

        // fade in screen
        image.sprite = controller.eventSystem.data.currentWeather.fadeBackgroundUI;
        LeanTween.move(rectTransform, new Vector2(0, 0), 0);
        LeanTween.alpha(rectTransform, 1f, 1f);
        LeanTween.alpha(rectTransform, 0f, 1f).setDelay(3f);
        LeanTween.move(rectTransform, new Vector2(0, 640f), 0).setDelay(4f);

        // fade in screen game day text
        text.text = "Day " + controller.timeSystem.currentInGameDay.ToString();
        text.color = controller.eventSystem.data.currentWeather.fadeInGameDayText;
        LeanTween.alphaText(fadeInGameDayText, 1f, 1f);
        LeanTween.alphaText(fadeInGameDayText, 0f, 1f).setDelay(3f);

        // farmtile movement
        var movementController = controller.movementsController;
        
        movementController.All_LeanTween_Start_Position(0f);
        movementController.All_LeanTween_Set_Position(2.7f);
    }
    public void Load_Day_Animation()
    {
        var image = nextDayFade.GetComponent<Image>();
        var text = fadeInGameDayText.GetComponent<Text>();
        var movementController = controller.movementsController;

        // fade screen
        image.sprite = controller.eventSystem.data.currentWeather.fadeBackgroundUI;
        LeanTween.move(nextDayFade, new Vector2(0, 0), 0);
        LeanTween.alpha(nextDayFade, 0f, 1f).setDelay(2f);
        LeanTween.move(nextDayFade, new Vector2(0, 640f), 0).setDelay(3f);
        
        // fade screen in game day text
        text.text = "Day " + controller.timeSystem.currentInGameDay.ToString();
        text.color = controller.eventSystem.data.currentWeather.fadeInGameDayText;
        LeanTween.alphaText(fadeInGameDayText, 0f, 1f).setDelay(2f);

        // farmtile movement
        movementController.All_Start_Position();
        movementController.All_LeanTween_Set_Position(1.7f);
    }

    // season and weather ui
    public void Season_UI_Update()
    {
        menuUI.seasonUIBox.sprite = controller.timeSystem.currentSeason.seasonUI;
        menuUI.seasonUIText.text = controller.timeSystem.currentSeason.seasonName;
    }
    public void Weather_UI_Update()
    {
        menuUI.weatherUIBox.sprite = controller.eventSystem.data.currentWeather.weatherUI;
        menuUI.weatherUIText.text = controller.eventSystem.data.currentWeather.weatherName;
    }

    // money system ui
    public void Money_Text_Update()
    {
        moneyText.text = "$ " + controller.money.ToString();
    }
    public void Money_Update_Fade_Tween(bool isAdd, int amount, int bonusAmount)
    {
        // fade original amount
        LeanTween.move(moneyFadeTextTransform, new Vector2(-4.899982f, -18.7f), 0.5f).setEase(moneyTweenType);
        LeanTween.alphaText(moneyFadeTextTransform, 0.7f, 0.5f);
        // return
        LeanTween.move(moneyFadeTextTransform, new Vector2(-4.899982f, -8.8f), 0f).setDelay(0.75f);
        LeanTween.alphaText(moneyFadeTextTransform, 0f, 0.25f).setDelay(0.5f);

        if (isAdd)
        {
            // original amount text
            moneyFadeText.text = "+$ " + amount.ToString();

            if (bonusAmount <= 0) return;

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
        else
        {
            moneyFadeText.text = "-$ " + amount.ToString();
            LeanTween.textColor(moneyFadeTextTransform, Color.red, 0);
            LeanTween.textColor(moneyFadeTextTransform, new Color32(30, 63, 45, 255), 0.2f).setDelay(0.31f);
        }
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
