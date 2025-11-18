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
        
        OnMenuUpdate += Update_IngredientMatchSlots;
        OnMenuUpdate += Update_IngredientSlots;

        base.OnEnable();

        Update_Slots();
        Update_IngredientMatchSlots();
        Update_Slider();
    }

    private new void OnDisable()
    {
        base.OnDisable();

        for (int i = 0; i < _cardSlots.Length; i++)
        {
            _cardSlots[i].Toggle_SelectHighlight(false);
        }

        OnDestroy();
    }

    private new void OnDestroy()
    {
        // subscriptions
        Input_Controller.instance.OnScroll -= Update_Slider;

        OnMenuUpdate -= Update_IngredientMatchSlots;
        OnMenuUpdate -= Update_IngredientSlots;

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

            slot.Update_AmountIndication(0);

            if (i > _menuCards.Length - 1)
            {
                slot.Empty_Slot();
                continue;
            }
            slot.Update_Slot(_menuCards[i]);
        }
    }


    private CardMenu_Slot CurrentSelected_Slot()
    {
        for (int i = 0; i < _cardSlots.Length; i++)
        {
            if (_cardSlots[i].selected == false) continue;
            return _cardSlots[i];
        }
        return null;
    }

    public void Select_Slot(CardMenu_Slot targetSlot)
    {
        for (int i = 0; i < _cardSlots.Length; i++)
        {
            CardMenu_Slot slot = _cardSlots[i];

            slot.Update_AmountIndication(0);

            if (targetSlot != slot)
            {
                slot.Toggle_SelectHighlight(false);
                continue;
            }
            slot.Toggle_SelectHighlight();
        }
    }


    public void Update_IngredientMatchSlots()
    {
        List<Card_IngredientData> ingredientDatas = Game_Controller.instance.AllCurrentCards_IngerdientConvertedDatas();

        for (int i = 0; i < _cardSlots.Length; i++)
        {
            CardMenu_Slot slot = _cardSlots[i];
            if (slot.currentCard == null) continue;

            slot.Toggle_Transparency(!slot.currentCard.IngredientDatas_Match(ingredientDatas));
        }
    }

    public void Update_IngredientSlots()
    {
        CardMenu_Slot selectedSlot = CurrentSelected_Slot();
        if (selectedSlot == null) return;

        Card_ScrObj slotCard = selectedSlot.currentCard;
        if (slotCard == null) return;

        List<Card_IngredientData> ingredients = slotCard.Card_IngredientDatas();
        if (ingredients.Count == 0) return;

        for (int i = 0; i < ingredients.Count; i++)
        {
            for (int j = 0; j < _cardSlots.Length; j++)
            {
                if (selectedSlot == _cardSlots[j]) continue;

                Card_ScrObj ingredientCard = ingredients[i].ingredientCard;
                if (ingredientCard != _cardSlots[j].currentCard) continue;

                int ingredientAmount = ingredients[i].amount;
                int currentAmount = Game_Controller.instance.CardType_CurrentAmount(ingredientCard);
                
                _cardSlots[j].Update_AmountIndication(ingredientAmount, currentAmount);
                _cardSlots[j].Update_AmountPanel(currentAmount >= ingredientAmount);

                break;
            }
        }
    }
}