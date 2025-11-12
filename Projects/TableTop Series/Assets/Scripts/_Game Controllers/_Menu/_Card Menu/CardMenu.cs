using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardMenu : Menu_Controller
{
    [Space(20)]
    [SerializeField] private Slider _slider;

    [Space(20)]
    [SerializeField] private Card_ScrObj[] _menuCards;
    [SerializeField] private CardMenu_Slot[] _cardSlots;


    // MonoBehaviour
    private new void OnEnable()
    {
        // subscriptions
        Input_Controller.instance.OnScroll += Update_Slider;
        
        base.OnEnable();

        Update_Slots();
        Update_Slider();
    }

    private new void OnDisable()
    {
        base.OnDisable();

        for (int i = 0; i < _cardSlots.Length; i++)
        {
            _cardSlots[i].Toggle_Highlight(false);
        }

        OnDestroy();
    }

    private new void OnDestroy()
    {
        // subscriptions
        Input_Controller.instance.OnScroll -= Update_Slider;

        base.OnDestroy();
    }


    // Slider
    private int Total_SlotPageCount()
    {
        int pageSlotsCount = _cardSlots.Length;
        return (_menuCards.Length + pageSlotsCount - 1) / pageSlotsCount;
    }

    private void Update_Slider()
    {
        int totalPageCount = Total_SlotPageCount();
        bool isSinglePage = totalPageCount <= 1;

        _slider.enabled = isSinglePage == false;
        _slider.handleRect.gameObject.SetActive(!isSinglePage);

        if (isSinglePage) return;

        _slider.maxValue = totalPageCount - 1;
        _slider.value = totalPageCount - 1;
    }
    private void Update_Slider(int scrollDirection)
    {
        int updateValue = (int)_slider.value + scrollDirection;
        _slider.value = Mathf.Clamp(updateValue, 0, Total_SlotPageCount() - 1);
    }


    // Slots
    private void Update_Slots()
    {
        for (int i = 0; i < _cardSlots.Length; i++)
        {
            CardMenu_Slot slot = _cardSlots[i];

            if (i > _menuCards.Length - 1)
            {
                slot.Empty_Slot();
                continue;
            }
            slot.Update_Slot(_menuCards[i]);
        }
    }

    public void Select_Slot(CardMenu_Slot targetSlot)
    {
        for (int i = 0; i < _cardSlots.Length; i++)
        {
            CardMenu_Slot slot = _cardSlots[i];
            
            if (targetSlot != slot)
            {
                slot.Toggle_Highlight(false);
                continue;
            }
            slot.Toggle_Highlight();
        }
    }
}