using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Buff")]
public class Buff_ScrObj : ScriptableObject
{
    public int buffID;
    public Sprite sprite;
    public string buffName;
    [TextArea(5, 10)]
    public string description;
    public int buffPrice;
}
