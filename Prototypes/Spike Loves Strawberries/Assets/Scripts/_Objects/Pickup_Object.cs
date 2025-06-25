using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Object : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private Interact_Controller _interaction;
    
    
    // MonoBehaviour
    public void Start()
    {
        _detection.OnPlayerDetect += () => _interaction.Toggle_Indication(true);
        _detection.OnPlayerExit += () => _interaction.Toggle_Indication(false);

        _interaction.OnInteract += Toggle_Pickup;
    }

    public void OnDestroy()
    {
        _interaction.OnInteract -= Toggle_Pickup;
    }
    
    
    // Pickup
    private void Toggle_Pickup()
    {
        Spike player = Level_Controller.instance.player;
        Spike_Data data = player.data;
        
        // drop
        if (data.hasItem && data.currentInteractable == gameObject)
        {
            data.Set_CurrentInteractable(null);
            transform.SetParent(Level_Controller.instance.transform);
            return;
        }

        // pickup
        if (data.hasItem) return;

        data.Set_CurrentInteractable(gameObject);
        
        transform.SetParent(player.headTransform);
        transform.localPosition = Vector2.zero;
    }
}
