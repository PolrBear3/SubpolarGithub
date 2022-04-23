using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Skill
{
    private string _name;
    public string name { get { return _name; } set { _name = value; } }

    private int _damage;
    public int damage { get { return _damage; } set { _damage = value; } }

    private float _areaOfEffect;
    public float areaOfEffect { get { return _areaOfEffect; } set { _areaOfEffect = value; } }

    public void Name(string name)
    {
        this.name = name;
    }

    public void Name_Damage(string name, int damage)
    {
        this.name = name;
        this.damage = damage;
    }

    public void All(string name, int damage, float areaOfEffect)
    {
        this.name = name;
        this.damage = damage;
        this.areaOfEffect = areaOfEffect;
    }

    public void Print_Ability_Stats()
    {
        Debug.Log(_name + " " + _damage + " " + _areaOfEffect);
    }
}
