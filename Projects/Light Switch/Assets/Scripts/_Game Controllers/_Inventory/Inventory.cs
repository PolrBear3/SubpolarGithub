using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Cursor _cursor;
    public Cursor cursor => _cursor;

    [SerializeField] private TextMeshProUGUI _totalToggleText;
    
    [Space(20)] 
    [SerializeField] private InventorySlot[] _inventorySlots;


    private Inventory_Data _data;
    public Inventory_Data data => _data;
    
    
    // MonoBehaviour
    private void Awake()
    {
        // load saved data
        _data = new();
    }

    private void Start()
    {
        // test
        Add_Switch(Main_Controller.instance.shop.purchasableSwitches[0]);
        
        Update_TotalToggleText();
    }


    // Indication
    public void Update_TotalToggleText()
    {
        _totalToggleText.text = _data.totalToggleCount.ToString();
    }


    // Main
    public void Pickup_Switch(InventorySlot slot)
    {
        Switch_SrcObj pickeUpSwitch = slot.currentSwitch;
        
        _cursor.image.color = pickeUpSwitch.switchColor;
        _cursor.Toggle_Activation(true);

        SwitchPlacer placer = Main_Controller.instance.switchPlacer;
        
        placer.Set_PlacingSwitch(pickeUpSwitch);
        cursor.OnLayerClick += placer.Cancel_PlacingSwitch;
        
        slot.Set_CurrentSwitch(null);
        slot.Load_SlotIndications();
    }

    public bool Add_Switch(Switch_SrcObj addingSwitch)
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (_inventorySlots[i].currentSwitch != null) continue;
            
            _inventorySlots[i].Set_CurrentSwitch(addingSwitch);
            _inventorySlots[i].Load_SlotIndications();
            
            return true;
        }
        return false;
    }
}