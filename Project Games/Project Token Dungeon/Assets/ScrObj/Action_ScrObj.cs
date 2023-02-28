using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Action")]
public class Action_ScrObj : ScriptableObject
{
    public int actionID;
    public string actionName;
    public float animationTime;
}
