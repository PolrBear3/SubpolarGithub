using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Default_Menu_ThemeUI
{
    public Image
    topMenuBox,
    moneyBox,
    backGround,
    nextDayButton,
    currentDayCountBox;

    // button pressed sprite
    public SpriteState spriteState;
}

public class Default_Menu : MonoBehaviour
{
    private Animator anim;

    public Default_Menu_ThemeUI themeUI;
    
    public MainGame_Controller controller;

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
        Weather_ThemeUI_AnimOverrideControl();
    }

    public void Update_UI()
    {
        Current_InGameDay_Text_Update();
        Next_Day_AlphaValueFade_Tween();
        Weather_ThemeUI_SpriteUpdate();
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
    private void Weather_ThemeUI_AnimOverrideControl()
    {
        anim.runtimeAnimatorController = controller.timeSystem.currentSeason.animatorOverrideController;

        var x = controller.eventSystem.currentWeather.weatherName;
        anim.SetTrigger(x);
    }
    private void Weather_ThemeUI_SpriteUpdate()
    {
        var x = controller.timeSystem.currentSeason.uiThemes;
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i].weatherNum == controller.eventSystem.currentWeather.weatherID)
            {
                themeUI.moneyBox.sprite = x[i].moneyBox;
                themeUI.topMenuBox.sprite = x[i].topMenuBox;
                themeUI.backGround.sprite = x[i].backGround;
                themeUI.nextDayButton.sprite = x[i].nextDayButton;
                themeUI.currentDayCountBox.sprite = x[i].currentDayCountBox;
            }
        }
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
