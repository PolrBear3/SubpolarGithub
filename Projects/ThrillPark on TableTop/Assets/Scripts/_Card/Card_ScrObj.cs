using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "ScriptableObject/ New Card")]
public class Card_ScrObj : ScriptableObject
{
    [Space(20)]
    [SerializeField] private string _cardName;
    public string cardName => _cardName;

    [SerializeField][TextArea(3, 5)] private string _cardDescription;
    public string cardDescription => _cardDescription;

    [Space(5)]
    [SerializeField] private LocalizedString _localizedCardName;
    [SerializeField] private LocalizedString _localizedDescription;

    [Space(20)]
    [SerializeField] private Sprite _iconSprite;
    public Sprite iconSprite => _iconSprite;

    [SerializeField] private Sprite _baseSprite;
    public Sprite baseSprite => _baseSprite;

    [Space(20)]
    [SerializeField] private Card_IngredientData[] _ingredientDatas;
    public Card_IngredientData[] ingredientDatas => _ingredientDatas;


    // Localization
    public string CardName()
    {
        if (_localizedCardName == null) return _cardName;
        if (string.IsNullOrEmpty(_localizedCardName.TableReference) && string.IsNullOrEmpty(_localizedCardName.TableEntryReference)) return _cardName;

        return _localizedCardName.GetLocalizedString();
    }

    public string CardDescription()
    {
        if (_localizedDescription == null) return _cardName;
        if (string.IsNullOrEmpty(_localizedDescription.TableReference) && string.IsNullOrEmpty(_localizedDescription.TableEntryReference)) return _cardDescription;

        return _localizedDescription.GetLocalizedString();
    }


    // Data
    private int CardIngredient_Amount(Card_ScrObj ingredientCard)
    {
        int totalAmount = 0;

        for (int i = 0; i < _ingredientDatas.Length; i++)
        {
            if (ingredientCard != _ingredientDatas[i].ingredientCard) continue;
            totalAmount += Mathf.Max(1, _ingredientDatas[i].amount);
        }

        return totalAmount;
    }

    public List<Card_IngredientData> Card_IngredientDatas()
    {
        List<Card_IngredientData> datas = new();

        for (int i = 0; i < _ingredientDatas.Length; i++)
        {
            Card_ScrObj dataCard = _ingredientDatas[i].ingredientCard;
            bool hasData = false;

            for (int j = 0; j < datas.Count; j++)
            {
                if (dataCard != datas[j].ingredientCard) continue;

                hasData = true;
                break;
            }

            if (hasData) continue;
            datas.Add(new(dataCard, CardIngredient_Amount(dataCard)));
        }

        return datas;
    }


    public int IngredientDatas_MatchCount(List<Card_IngredientData> ingredients)
    {
        List<Card_IngredientData> currentDatas = Card_IngredientDatas();
        int matchCount = 0;

        for (int i = 0; i < currentDatas.Count; i++)
        {
            Card_ScrObj currentCard = currentDatas[i].ingredientCard;
            int amount = currentDatas[i].amount;

            for (int j = 0; j < ingredients.Count; j++)
            {
                if (ingredients[j].ingredientCard != currentCard) continue;
                amount -= ingredients[j].amount;

                if (amount > 0) continue;

                matchCount++;
                break;
            }
        }

        return matchCount;
    }

    public bool IngredientDatas_Match(List<Card_IngredientData> ingredients)
    {
        return Card_IngredientDatas().Count == IngredientDatas_MatchCount(ingredients);
    }
}
