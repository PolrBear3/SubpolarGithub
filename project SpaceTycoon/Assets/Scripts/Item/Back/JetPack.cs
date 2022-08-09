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

    [HideInInspector]
    public bool outOfFuel = false;

    private void Update()
    {
        Button_Press_Function();
        JetPack_Active_Use();
        Outof_Fuel();
    }
    private void OnEnable()
    {
        DeActivate_DefaultJump();
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

    private void Use_Fuel()
    {
        gameObjectItemInfo.equipSlot.currentDurability -= 1 * Time.deltaTime;
    }
    private void Outof_Fuel()
    {
        if (gameObjectItemInfo.equipSlot.currentDurability <= 0f)
        {
            gameObjectItemInfo.equipSlot.currentDurability = 0f;
            Activate_DefaultJump(); 
            outOfFuel = true;
        }
        else
        {
            DeActivate_DefaultJump();
            outOfFuel = false;
        }
    }
}
