using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Monster
{
    private int health;

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }
}
