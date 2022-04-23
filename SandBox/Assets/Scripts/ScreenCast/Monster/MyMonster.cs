using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMonster : MonoBehaviour
{
    public int damage;
    public int health;

    private Monster monster;

    private void Start()
    {
        monster = new Monster();
        monster.SetHealth(health);
    }

    public void Damage_Monster()
    {
        monster.SetHealth(monster.GetHealth() - damage);
        Debug.Log(monster.GetHealth());
    }
}
