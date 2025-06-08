using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDrop : ItemDrop
{
    [Space(20)]
    [SerializeField] private CoinLauncher _launcher;

    [Space(20)] 
    [SerializeField] private Vector2 _defaultDropAmount;
    
    [Space(60)] 
    [SerializeField] private Ability_ScrObj _goldMagnetAbility;
    
    
    private GoldSystem_Data _data;
    
    
    // UnityEngine
    private new void Start()
    {
        base.Start();
        
        interactable.OnInteract += Pickup;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        
        interactable.OnInteract -= Pickup;
    }
    
    
    //
    public void Set_Data(int dropAmount)
    {
        _data = new(dropAmount);
    }
    
    private void Pickup()
    {
        if (_data == null)
        {
            int dropAmount = Random.Range((int)_defaultDropAmount.x, (int)_defaultDropAmount.y);
            Set_Data(dropAmount);
        }
        
        GoldSystem.instance.Update_CurrentAmount(_data.goldAmount);
        AbilityManager.IncreasePoint(_goldMagnetAbility, 1);
        
        // effects
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
        _launcher.Parabola_CoinLaunch(_launcher.setCoinSprites[0], detection.player.transform.position);
        
        Destroy(gameObject, 0.1f);
    }
}
