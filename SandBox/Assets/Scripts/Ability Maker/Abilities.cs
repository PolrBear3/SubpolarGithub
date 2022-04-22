using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    Skill IceBall = new Skill();
    Skill FireBall = new Skill();
    Skill EarthShake = new Skill();

    private void Start()
    {
        IceBall.Name("Iceball");
        FireBall.Name_Damage("Fireball", 100);
        EarthShake.All("Earth Shake", 10, 100.0f);
    }

    public void Print_Ability_Stats()
    {
        IceBall.Print_Ability_Stats();
        FireBall.Print_Ability_Stats();
        EarthShake.Print_Ability_Stats();
    }
}
