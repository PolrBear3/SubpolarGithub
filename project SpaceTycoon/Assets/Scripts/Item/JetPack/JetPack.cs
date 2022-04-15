using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : MonoBehaviour
{
    private void Start()
    {
        Set_Max_Fuel();
    }

    private void Update()
    {
        Button_Press_Function();
        Outof_Fuel_Function();
    }

    public Animator anim;

    public float flyForce;
    [HideInInspector]
    public bool buttonPressed;

    void Button_Press_Function()
    {
        if (buttonPressed)
        {
            anim.SetBool("isPressed", true);
            Use_Fuel();
        }
        else
        {
            anim.SetBool("isPressed", false);
        }
    }

    float maxEnergyFuel = 50f;
    float currentEnergyFuel;
    [HideInInspector]
    public bool outOfFuel = false;

    void Set_Max_Fuel()
    {
        currentEnergyFuel = maxEnergyFuel;
    }
    void Outof_Fuel_Function()
    {
        if (currentEnergyFuel <= 0f)
        {
            currentEnergyFuel = 0f;
            outOfFuel = true;
        }
    }
    void Use_Fuel()
    {
        currentEnergyFuel -= 1 * Time.deltaTime;
    }
}
