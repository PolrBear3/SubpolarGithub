using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Weather_Data
{
    public Weather_ScrObj weather;
    public float percentage;
}

[CreateAssetMenu(menuName = "New Season")]
public class Season_ScrObj : ScriptableObject
{
    public Weather_Data[] weathers;

    public string seasonName;
    public int seasonID;
    public Sprite seasonUI;
    public Sprite backGroundUI;
    public Sprite tileBorder;
}
