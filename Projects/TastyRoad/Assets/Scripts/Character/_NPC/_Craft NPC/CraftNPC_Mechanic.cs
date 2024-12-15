using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Mechanic : CraftNPC
{
    // OnSetInstance
    public void Subscribe_Interactions()
    {
        ActionBubble_Interactable interactable = controller.controller.interactable;

        interactable.OnIInteract += Interact_Check;
    }


    //
    private void Interact_Check()
    {
        Debug.Log("CraftNPC_Mechanic");
    }
}