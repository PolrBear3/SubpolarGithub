using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu (menuName = "New Seed")]
public class Seed_ScrObj : ScriptableObject
{
    public int seedID;
    public Sprite sprite;
    public int minFinishDays, maxFinishDays;
}
