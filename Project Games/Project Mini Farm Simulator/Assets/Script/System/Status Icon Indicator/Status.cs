using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType
{
    watered,
    damaged,
    sunnyBuffed,
    cloudyStunned
}

[CreateAssetMenu(menuName = "New Status")]
public class Status : ScriptableObject
{
    public StatusType statusType;
    public Sprite statusIcon;
}
