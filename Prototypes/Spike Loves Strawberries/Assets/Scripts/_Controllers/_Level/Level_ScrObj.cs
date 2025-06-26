using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Level")]
public class Level_ScrObj : ScriptableObject
{
    [Space(20)]
    public string levelName;
    public GameObject _levelPrefab;
}
