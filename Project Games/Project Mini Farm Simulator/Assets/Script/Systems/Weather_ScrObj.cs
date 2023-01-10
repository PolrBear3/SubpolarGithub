using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "New Weather")]
public class Weather_ScrObj : ScriptableObject
{
    public int weatherID;
    public string weatherName;

    public Sprite weatherUI;
    public Sprite fadeBackgroundUI;
}
