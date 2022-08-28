using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Default_Menu : MonoBehaviour
{
    private Animator anim;
    
    public MainGame_Controller controller;

    public Image currentBackground;

    public GameObject nextDayFade;
    public Text currentInGameDayText, moneyText;

    public RectTransform moneyFadeTextTransform;
    public Text moneyFadeText;

    public RectTransform bonusFadeTextTransform;
    public Text bonusFadeText;

    public LeanTweenType moneyTweenType;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        Money_Text_Update();
    }
    private void Update()
    {
        Weather_ThemeUI_Update();
    }

    public void Update_UI()
    {
        Current_InGameDay_Text_Update();
        Next_Day_AlphaValueFade_Tween();
        Weather_Background_Color_Set();
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

    // season weather ui theme
    private void Weather_Background_Color_Set()
    {
        var x = controller.eventSystem.currentWeather.weatherID;
        var y = controller.timeSystem.currentSeason.backgroundThemeColors;

        if (x == 0) { currentBackground.sprite = y[0]; }
        else if (x == 1) { currentBackground.sprite = y[1]; }
        else if (x == 1) { currentBackground.sprite = y[2]; }
        else if (x == 1) { currentBackground.sprite = y[3]; }
    }
    private void Weather_ThemeUI_Update()
    {
        anim.runtimeAnimatorController = controller.timeSystem.currentSeason.defaultMenuAnimation;

        var x = controller.eventSystem.currentWeather.weatherID;

        if (x == 0) { anim.SetTrigger("sunny"); }
        else if (x == 1) { anim.SetTrigger("cloudy"); }
        else if (x == 2) { anim.SetTrigger("rainy"); }
        else if (x == 3) { anim.SetTrigger("stormy"); }
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
