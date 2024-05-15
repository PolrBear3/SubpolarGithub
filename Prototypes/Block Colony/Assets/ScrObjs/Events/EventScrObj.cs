using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Event")]
public class EventScrObj : ScriptableObject
{
    public int sortingLayerNum;

    public AnimatorOverrideController eventAnimation;

    [TextArea(3, 10)]
    public string description;
}
