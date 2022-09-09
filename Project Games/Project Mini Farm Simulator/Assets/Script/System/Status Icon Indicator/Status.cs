using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType
{
    watered
}

[CreateAssetMenu(menuName = "New Status")]
public class Status : ScriptableObject
{
    public StatusType statusType;
    public Sprite statusIcon;
}
