using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Switch_SrcObj[] _purchasableSwitches;
    public Switch_SrcObj[] purchasableSwitches => _purchasableSwitches;
    
    [SerializeField] private PurchaseSlot[] _purchaseSlots;
    
    
    // MonoBehaviour
    private void Start()
    {
        Main_Controller.instance.invokeExecutionController.Subscribe_Action(Load_PurchasableSwitches, 0);
    }

    private void OnDestroy()
    {
        Main_Controller.instance.invokeExecutionController.Unsubscribe_Action(Load_PurchasableSwitches, 0);
    }


    // Main
    public void Purchase_Switch(PurchaseSlot purchaseSlot)
    {
        Switch_SrcObj purchaseSwitch = purchaseSlot.purchasableSwitch;
        Inventory inventory = Main_Controller.instance.inventory;

        if (inventory.data.totalToggleCount < purchaseSwitch.price) return;
        if (inventory.Add_Switch(purchaseSwitch) == false) return;
        
        inventory.data.Set_TotalToggleCount(inventory.data.totalToggleCount - purchaseSwitch.price);
        inventory.Update_TotalToggleText();
        
        Update_PurchaseSlots();
    }
    
    
    // Purchase Slots
    private void Load_PurchasableSwitches()
    {
        for (int i = 0; i < _purchaseSlots.Length; i++)
        {
            Switch_SrcObj setSwitch = i <= _purchasableSwitches.Length - 1 ? _purchasableSwitches[i] : null;
            
            _purchaseSlots[i].Set_PurchaseSwitch(setSwitch);
            _purchaseSlots[i].Load_PurchaseIndications();
        }
    }

    private void Update_PurchaseSlots()
    {
        
    }
}
