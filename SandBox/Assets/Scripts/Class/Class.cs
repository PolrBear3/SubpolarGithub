using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss
{
    private string _name;
    public string name { get { return _name; } set { _name = value; } }

    private int _health;
    public int health { get { return _health; } set { _health = value; } }

    public enum type { earth, water, fire}

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public virtual void Take_Damage(int damage, type type)
    {
        _health -= damage;
    }

    public void Show_Current_Health()
    {
        Debug.Log(_health);
    }
}

public class EarthBoss : Boss
{
    public override void Take_Damage(int damage, type type)
    {
        if (type == type.fire)
        {
            base.Take_Damage(damage * 2, type);
        }
        if (type == type.water)
        {
            base.Take_Damage(damage / 2, type);
        }
    }
}

public class WaterBoss : Boss
{
    public override void Take_Damage(int damage, type type)
    {
        if (type == type.fire)
        {
            base.Take_Damage(damage / 2, type);
        }
        if (type == type.earth)
        {
            base.Take_Damage(damage * 2, type);
        }
    }
}

public class FireBoss : Boss
{
    public override void Take_Damage(int damage, type type)
    {
        if (type == type.water)
        {
            base.Take_Damage(damage * 2, type);
        }
        if (type == type.earth)
        {
            base.Take_Damage(damage / 2, type);
        }
    }
}
