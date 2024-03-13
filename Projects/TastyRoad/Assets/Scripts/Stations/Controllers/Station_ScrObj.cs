using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Station!")]
public class Station_ScrObj : ScriptableObject
{
    public GameObject prefab;

    public Sprite miniSprite;
    public Vector2 centerPosition;

    public bool unRetrievable;

    public string stationName;
    public int id;

    public int price;
}