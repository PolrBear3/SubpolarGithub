using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Behaviour : MonoBehaviour
{
    [Header("")]
    [SerializeField] private AbilityManager _manager;
    public AbilityManager manager => _manager;

    [SerializeField] private Ability_ScrObj _abilityScrObj;
    public Ability_ScrObj abilityScrObj => _abilityScrObj;
}