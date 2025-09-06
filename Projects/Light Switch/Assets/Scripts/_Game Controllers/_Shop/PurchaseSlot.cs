using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseSlot : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Image _slotPanel;
    public Image SlotPanel => _slotPanel;
    
    [SerializeField] private Image _switchIconImage;
    public Image SwitchIconImage => _switchIconImage;

    [Space(20)] 
    [SerializeField] private Color _emptyPanelColor;
    // [SerializeField] private Sprite _emptyPanelSprite;
    

    // PurchaseSlot_Data
    private Switch_SrcObj _purchasableSwitch;
    public Switch_SrcObj purchasableSwitch => _purchasableSwitch;
    
    
    // UI Event System
    public void Purchase_CurrentSwitch()
    {
        if (_purchasableSwitch == null) return;
        Main_Controller.instance.shop.Purchase_Switch(this);
    }
    
    
    // Data
    public void Set_PurchaseSwitch(Switch_SrcObj setSwitch)
    {
        _purchasableSwitch = setSwitch;
    }
    
    
    // Indication
    public void Load_PurchaseIndications()
    {
        if (_purchasableSwitch == null)
        {
            _slotPanel.color = _emptyPanelColor;
            _switchIconImage.color = Color.clear;
            
            return;
        }
        
        _slotPanel.color = Color.white;
        _switchIconImage.color = _purchasableSwitch.switchColor;
    }
}
