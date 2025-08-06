using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Location!")]
public class Location_ScrObj : ScriptableObject
{
    [Space(20)]
    public int worldNum;
    public int locationNum;

    [Space(20)]
    public GameObject locationPrefab;
}
