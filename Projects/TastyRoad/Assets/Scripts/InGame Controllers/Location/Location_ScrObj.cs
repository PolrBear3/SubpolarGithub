using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Location!")]
public class Location_ScrObj : ScriptableObject
{
    public int worldNum;
    public int locationNum;

    public GameObject locationPrefab;
}
