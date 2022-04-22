using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minor_Class : MonoBehaviour
{
    EarthBoss earthBoss = new EarthBoss();
    WaterBoss waterBoss = new WaterBoss();
    FireBoss fireBoss = new FireBoss();

    private void Start()
    {
        earthBoss.SetHealth(200);
        waterBoss.SetHealth(150);
        fireBoss.SetHealth(100);
    }

    public void EarthBoss_Take_Damge()
    {
        earthBoss.Take_Damage(10, Boss.type.fire);
        earthBoss.Show_Current_Health();
    }

    public void WaterBoss_Take_Damage()
    {
        waterBoss.Take_Damage(10, Boss.type.earth);
        waterBoss.Show_Current_Health();
    }

    public void FireBoss_Take_Damage()
    {
        fireBoss.Take_Damage(10, Boss.type.water);
        fireBoss.Show_Current_Health();
    }
}