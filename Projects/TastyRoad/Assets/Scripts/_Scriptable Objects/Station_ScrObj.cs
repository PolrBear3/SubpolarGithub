using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Station!")]
public class Station_ScrObj : ScriptableObject
{
    [Header("")]
    public GameObject prefab;

    [Header("")]
    public Sprite sprite;
    public Sprite miniSprite;
    public Sprite dialogIcon;

    [Header("")]
    public Vector2 centerPosition;

    [Header("")]
    public string stationName;
    public int id;

    [Header("")]
    [Range(0, 100)] public int price;
    [Range(0, 100)] public int buildToArchiveCount;

    [Header("")]
    [Range(0, 100)] public int durability;
}