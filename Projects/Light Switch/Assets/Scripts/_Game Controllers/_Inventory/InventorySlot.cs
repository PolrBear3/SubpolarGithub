using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Image _switchIconImage;
    
    
    // InventorySlot_Data
    private Switch_SrcObj _currentSwitch;
    public Switch_SrcObj currentSwitch => _currentSwitch;
    
    
    // UI Event System
    public void Pickup_CurrentSwitch()
    {
        if (_currentSwitch == null) return;
        Main_Controller.instance.inventory.Pickup_Switch(this);
    }
    
    
    // Data
    public void Set_CurrentSwitch(Switch_SrcObj setSwitch)
    {
        _currentSwitch = setSwitch;
    }
    
    
    // Indication
    public void Load_SlotIndications()
    {
        Color iconColor = _currentSwitch != null ? _currentSwitch.switchColor : Color.white;
        _switchIconImage.color = iconColor;
    }
}
