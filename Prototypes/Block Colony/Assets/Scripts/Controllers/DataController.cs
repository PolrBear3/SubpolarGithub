using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _landPrefab;
    public GameObject landPrefab => _landPrefab;

    [SerializeField] private GameObject _cardPrefab;
    public GameObject cardPrefab => _cardPrefab;

    [Header("")]
    [SerializeField] private CardScrObj[] _allCardScrObjs;
    public CardScrObj[] allCardScrObj => _allCardScrObjs;

    [SerializeField] private CardScrObj[] _allLandCardsScrObj;
    public CardScrObj[] allLandCardsScrObj => _allLandCardsScrObj;

    [SerializeField] private CardScrObj[] _allBuffCardScrObjs;
    public CardScrObj[] allBuffCardScrObjs => _allBuffCardScrObjs;


    // Get Cards
    public List<CardScrObj> AllCard_ScrObjs(int amount)
    {
        List<CardScrObj> allCards = new();

        for (int i = 0; i < amount; i++)
        {
            allCards.Add(_allCardScrObjs[Random.Range(0, _allCardScrObjs.Length)]);
        }

        return allCards;
    }

    public List<CardScrObj> AllLandCard_ScrObjs(int amount)
    {
        List<CardScrObj> allCards = new();

        for (int i = 0; i < amount; i++)
        {
            allCards.Add(_allLandCardsScrObj[Random.Range(0, _allLandCardsScrObj.Length)]);
        }

        return allCards;
    }

    public List<CardScrObj> AllBuffCard_ScrObjs(int amount)
    {
        List<CardScrObj> allCards = new();

        for (int i = 0; i < amount; i++)
        {
            allCards.Add(_allBuffCardScrObjs[Random.Range(0, _allBuffCardScrObjs.Length)]);
        }

        return allCards;
    }
}
