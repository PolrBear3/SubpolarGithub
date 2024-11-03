using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Ability!")]
public class Ability_ScrObj : ScriptableObject
{
    [SerializeField] private string _abilityName;
    public string abilityName => _abilityName;
}
