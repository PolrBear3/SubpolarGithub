using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    public static Game_Controller instance;
    
    [Space(20)]
    [SerializeField] private Camera _mainCamera;
    public Camera mainCamera => _mainCamera;
    
    [Space(20)]
    [SerializeField] private Cursor _cursor;
    public Cursor cursor => _cursor;
    
    [SerializeField] private TableTop _tableTop;
    public TableTop tableTop => _tableTop;

    [SerializeField] private GameMenu_Controller _gameMenu;
    public GameMenu_Controller gameMenu => _gameMenu;


    // MonoBehaviour
    private void Awake()
    {
        instance = this;
    }


    // Main Data
    public List<Card_Data> AllCurrent_CardDatas()
    {
        List<Card_Data> allDatas = new();

        List<Card_Data> tableTopDatas = _tableTop.CurrentCards_Datas();
        List<Card_Data> dragDatas = _cursor.currentCardDatas;

        foreach (Card_Data data in tableTopDatas)
        {
            allDatas.Add(data);
        }
        foreach (Card_Data data in dragDatas)
        {
            allDatas.Add(data);
        }

        return allDatas;
    }

    public List<Card_IngredientData> AllCurrentCards_IngerdientConvertedDatas()
    {
        List<Card_Data> currentDatas = AllCurrent_CardDatas();
        List<Card_IngredientData> ingredientDatas = new();

        for (int i = 0; i < currentDatas.Count; i++)
        {
            Card_ScrObj currentCard = currentDatas[i].cardScrObj;
            bool hasData = false;

            for (int j = 0; j < ingredientDatas.Count; j++)
            {
                if (currentCard != ingredientDatas[j].ingredientCard) continue;
                
                ingredientDatas[j] = new(currentCard, ingredientDatas[j].amount + 1);
                hasData = true;

                break;
            }

            if (hasData) continue;
            ingredientDatas.Add(new(currentCard, 1));
        }

        return ingredientDatas;
    }

    public int CardType_CurrentAmount(Card_ScrObj targetCard)
    {
        int amount = 0;

        List<Card_Data> currentDatas = AllCurrent_CardDatas();

        for (int i = 0; i < currentDatas.Count; i++)
        {
            if (targetCard != currentDatas[i].cardScrObj) continue;
            amount++;
        }

        return amount;
    }
}
