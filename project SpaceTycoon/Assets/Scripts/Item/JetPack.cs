using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : MonoBehaviour
{   
    private void Start()
    {
        Set_Current_Fuel();
    }

    private void Update()
    {
        Set_Max_Fuel();
        Button_Press_Function();
        Outof_Fuel_Function();
        Debug.Log(currentEnergyFuel);
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
    [HideInInspector]
    public static float currentEnergyFuel;
    [HideInInspector]
    public bool outOfFuel = false;

    void Set_Max_Fuel()
    {
        if (currentEnergyFuel >= maxEnergyFuel)
        {
            currentEnergyFuel = maxEnergyFuel;
        }
    }
    void Set_Current_Fuel()
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
