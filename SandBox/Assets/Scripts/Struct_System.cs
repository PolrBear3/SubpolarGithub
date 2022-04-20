using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Weapon
{
    public string name;
    public int damage;
    public int price;
}

public class Struct_System : MonoBehaviour
{
    private void Start()
    {
        Set_New_Weapon();
    }

    Weapon[] weapons = new Weapon[3];

    void Set_New_Weapon()
    {
        weapons[0].name = "sword";
        weapons[0].damage = 5;
        weapons[0].price = 1;

        weapons[1].name = "arrow";
        weapons[1].damage = 2;
        weapons[1].price = 3;

        weapons[2].name = "hammer";
        weapons[2].damage = 8;
        weapons[2].price = 4;
    }

    public void Print_Weapon_Info()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            Debug.Log(weapons[i].name);
            Debug.Log(weapons[i].damage);
            Debug.Log(weapons[i].price);
        }
    }
}
