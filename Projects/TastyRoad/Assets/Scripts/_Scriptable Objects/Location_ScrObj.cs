using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Location!")]
public class Location_ScrObj : ScriptableObject
{
    public int worldNum;
    public int locationNum;

    public string locationNickName;

    public Sprite locationIcon;

    public GameObject locationPrefab;
}
