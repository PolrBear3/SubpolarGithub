using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berry : Pickup_Object
{
    // MonoBehaviour
    public new void Start()
    {
        Set_OnCurrentPlatform();
        
        detection.OnPlayerDetect += Update_Indication;
        detection.OnPlayerExit += Update_Indication;

        interaction.OnInteract += Update_Pickup;
    }

    public new void OnDestroy()
    {
        detection.OnPlayerDetect -= Update_Indication;
        detection.OnPlayerExit -= Update_Indication;
        
        interaction.OnInteract -= Update_Pickup;
    }
    
    
    // Interaction
    private void Update_Pickup()
    {
        Spike_Data playerData = Level_Controller.instance.player.data;

        if (playerData.hasTail == false) 
        {
            Eat();
            return;
        }
        
        Toggle_Pickup();
    }

    private void Eat()
    {
        Spike_Data playerData = Level_Controller.instance.player.data;
        bool hadItem = playerData.hasItem;

        Level_Controller.instance.player.Toggle_TailDetachment(true);
        Destroy(gameObject);

        if (hadItem == false) return;
        playerData.Set_CurrentInteractable(null);
    }
}
