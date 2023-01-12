using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu (menuName = "New Seed")]
public class Seed_ScrObj : ScriptableObject
{
    public int seedID;
    public Sprite[] sprites;

    public int seedHealth;
    public int waterHealth;
    public int startBonusPoints;

    // set min and max range around 5
    public int minFinishDays, maxFinishDays;
    public int seedBuyPrice, harvestSellPrice;
    public float waitTime;

    // seed detail for tooltip
    public string seedName;
    [TextArea(5, 10)]
    public string seedDescription;
}
