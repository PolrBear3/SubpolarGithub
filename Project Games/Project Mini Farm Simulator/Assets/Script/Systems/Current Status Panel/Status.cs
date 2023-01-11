using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Status")]
public class Status : ScriptableObject
{
    public int statusID;
    public Sprite statusIcon;
    [TextArea (5, 5)]
    public string statusDescription;

    public int healthValue;
}
