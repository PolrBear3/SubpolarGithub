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
        Button_Press_Function();
        Limit_Current_Fuel();
        Outof_Fuel();
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

    public BackSlot backSlot;
    public float maxEnergyFuel;
    [HideInInspector]
    public float currentEnergyFuel;
    [HideInInspector]
    public bool outOfFuel = false;

    void Set_Current_Fuel()
    {
        currentEnergyFuel = maxEnergyFuel;
    }
    void Limit_Current_Fuel()
    {
        if (currentEnergyFuel > maxEnergyFuel)
        {
            currentEnergyFuel = maxEnergyFuel;
        }
    }
    void Use_Fuel()
    {
        currentEnergyFuel -= 1 * Time.deltaTime;
    }
    void Outof_Fuel()
    {
        if (currentEnergyFuel <= 0f)
        {
            currentEnergyFuel = 0f;
            outOfFuel = true;
        }
    }
}
