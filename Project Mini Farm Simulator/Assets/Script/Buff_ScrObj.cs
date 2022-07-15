using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { normalStartBooster, normalUpdateBooster, specialBooster}

[CreateAssetMenu(menuName = "New Buff")]
public class Buff_ScrObj : ScriptableObject
{
    public int buffID;
    public Sprite sprite;
    public int buffPrice;
    public BuffType buffType;

    public int startAdvantageDayPoint;

    public int updateAdvantageDayPoint;
    public int updateAdvantagePercentage;
}
