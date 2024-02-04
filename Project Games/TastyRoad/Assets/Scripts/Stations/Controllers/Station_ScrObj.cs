using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Station!")]
public class Station_ScrObj : ScriptableObject
{
    public Sprite sprite;

    public Vector2 centerPosition;

    public string stationName;
    public int id;

    public int price;
}