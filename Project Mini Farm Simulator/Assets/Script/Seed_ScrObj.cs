using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "New Seed")]
public class Seed_ScrObj : ScriptableObject
{
    public int seedID;
    public string seedName;
    public int minFinishDays, maxFinishDays;
}
