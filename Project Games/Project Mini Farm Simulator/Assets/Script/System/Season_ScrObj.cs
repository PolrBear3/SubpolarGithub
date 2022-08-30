using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UITheme
{
    public int weatherNum;

    public Sprite 
    backGround, 
    nextDayButton,
    currentDayCountBox;     
}

[CreateAssetMenu(menuName = "New Season")]
public class Season_ScrObj : ScriptableObject
{
    public Weather_ScrObj[] weatherPercentages;
    
    public AnimatorOverrideController animatorOverrideController;
    public UITheme[] uiThemes;
}
