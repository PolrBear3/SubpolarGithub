using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : LoopMovement_Object
{
    [Space(20)] 
    [SerializeField] private Detection_Controller _detection;
    
    
    // MonoBehaviour
    private new void Start()
    {
        base.Start();
        
        // subscriptions
        _detection.OnPlayerDetect += Update_PlayerDamage;
        _detection.OnPlayerExit += Level_Controller.instance.player.Cancel_Damage;
    }

    private new void OnDestroy()
    {
        // subscriptions
        _detection.OnPlayerDetect -= Update_PlayerDamage;
        _detection.OnPlayerExit -= Level_Controller.instance.player.Cancel_Damage;
    }
    
    
    // Damage
    private void Update_PlayerDamage()
    {
        if (_detection.playerDetected == false) return;
        Spike detectedPlayer = Level_Controller.instance.player;

        detectedPlayer.Update_Damage();
        
        if (detectedPlayer.data.hasTail == false) return;
        detectedPlayer.Toggle_TailDetachment(false);
    }
}
