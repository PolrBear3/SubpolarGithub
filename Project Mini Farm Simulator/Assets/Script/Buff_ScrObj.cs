using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Buff")]
public class Buff_ScrObj : ScriptableObject
{
    public int buffID;
    public Sprite sprite;

    public int buffPrice;
}
