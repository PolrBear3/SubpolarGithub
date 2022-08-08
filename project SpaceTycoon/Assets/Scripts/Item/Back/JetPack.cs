using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : MonoBehaviour
{
    public Player_MainController playerController;
    public GameObject_Item_Info gameObjectItemInfo;
    public Animator anim;

    public float flyForce;
    [HideInInspector]
    public bool buttonPressed;

    public float maxEnergyFuel;
    public float currentEnergyFuel;
    [HideInInspector]
    public bool outOfFuel = false;

    private void Update()
    {
        Button_Press_Function();
        Limit_Current_Fuel();
        Outof_Fuel();

        JetPack_Active_Use();
    }
    private void OnEnable()
    {
        DeActivate_DefaultJump();
        Set_Max_Fuel();
        Set_Current_Fuel();
    }
    private void OnDisable()
    {
        Activate_DefaultJump();
    }

    private void Activate_DefaultJump()
    {
        playerController.playerMovement.defaultJumpOn = true;
    }
    private void DeActivate_DefaultJump()
    {
        playerController.playerMovement.defaultJumpOn = false;
    }

    private void JetPack_Active_Use()
    {
        var playerMovement = playerController.playerMovement;
        
        if (!outOfFuel && Input.GetKey(KeyCode.W))
        {
            buttonPressed = true;
            playerMovement.rb.AddForce(Vector2.up * flyForce);
        }
        else
        {
            buttonPressed = false;
        }
    }
    private void Button_Press_Function()
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
    
    private void Set_Max_Fuel()
    {
        var equipedJetPackMaxDurability = gameObjectItemInfo.equipSlot.currentItem.itemMaxDurability;
        maxEnergyFuel = equipedJetPackMaxDurability;
    }
    private void Set_Current_Fuel()
    {
        var equipJetPackDurability = gameObjectItemInfo.equipSlot.currentDurability;
        currentEnergyFuel = equipJetPackDurability;
    }

    private void Limit_Current_Fuel()
    {
        if (currentEnergyFuel > maxEnergyFuel)
        {
            currentEnergyFuel = maxEnergyFuel;
        }
    }
    private void Use_Fuel()
    {
        currentEnergyFuel -= 1 * Time.deltaTime;
        gameObjectItemInfo.equipSlot.currentDurability = currentEnergyFuel;
    }
    private void Outof_Fuel()
    {
        if (currentEnergyFuel <= 0f)
        {
            currentEnergyFuel = 0f;
            outOfFuel = true;
        }
    }
}
