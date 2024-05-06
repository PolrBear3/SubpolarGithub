using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Event")]
public class EventScrObj : ScriptableObject
{
    public int id;
    public string eventName;

    public Sprite icon;
    public Sprite landSprite;

    [TextArea(3, 10)]
    public string description;
}
