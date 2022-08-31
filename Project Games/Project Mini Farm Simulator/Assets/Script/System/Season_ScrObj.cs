using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UITheme
{
    public int weatherNum;

    public Sprite
    topMenuBox,
    moneyBox,
    backGround, 
    nextDayButton,
    currentDayCountBox;

    // button pressed sprite
    public SpriteState spriteState;
}

[CreateAssetMenu(menuName = "New Season")]
public class Season_ScrObj : ScriptableObject
{
    public Weather_ScrObj[] weatherPercentages;
    
    public AnimatorOverrideController animatorOverrideController;
    public UITheme[] uiThemes;
}
