using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleOrder_Ability : Ability_Behaviour, IAbility
{
    // UnityEngine
    private new void Start()
    {
        Main_Controller.instance.worldMap.OnLocationSet += Load_Activation;
    }

    private new void OnDestroy()
    {
        Main_Controller.instance.worldMap.OnLocationSet -= Load_Activation;
    }
    
    
    // IAbility
    public void Activate()
    {
        Location_Controller currentLocation = Main_Controller.instance.currentLocation;
        if (currentLocation == null) return;
        
        currentLocation.data.UpdateCurrent_FoodOrderCount(1);
    }

    private void Load_Activation()
    {
        int activationCount = manager.data.AbilityData(abilityScrObj).activationCount;
        
        Location_Controller currentLocation = Main_Controller.instance.currentLocation;
        currentLocation.data.UpdateCurrent_FoodOrderCount(activationCount);
    }
}
