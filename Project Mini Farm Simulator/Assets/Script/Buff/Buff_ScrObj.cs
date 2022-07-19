using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { normalStartBooster, normalUpdateBooster, specialBooster}

[CreateAssetMenu(menuName = "New Buff")]
public class Buff_ScrObj : ScriptableObject
{
    public int buffID;
    public Sprite sprite;
    public string buffName;
    public BuffType buffType;
    [TextArea(5, 10)]
    public string description;
    public int buffPrice;

    public int startAdvantageDayPoint;

    public int updateAdvantageDayPoint;
    public int updateAdvantagePercentage;
}
