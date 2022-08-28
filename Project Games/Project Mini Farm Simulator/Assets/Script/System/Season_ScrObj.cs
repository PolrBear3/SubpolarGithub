using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "New Season")]
public class Season_ScrObj : ScriptableObject
{
    public AnimatorOverrideController defaultMenuAnimation;

    public Sprite[] weatherIcons;
    public Sprite[] backgroundThemeColors;

    public Weather_ScrObj[] weatherPercentages;
}
