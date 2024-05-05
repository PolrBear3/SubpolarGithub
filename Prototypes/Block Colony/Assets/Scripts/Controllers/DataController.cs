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


    //



    /// <returns>
    /// A Random Card ScrObj
    /// </returns>
    public CardScrObj Card_ScrObj()
    {
        return _allCardScrObjs[Random.Range(0, _allCardScrObjs.Length)];
    }

    /// <returns>
    /// A Card ScrObj according to id
    /// </returns>
    public CardScrObj Card_ScrObj(int id)
    {
        for (int i = 0; i < _allCardScrObjs.Length; i++)
        {
            if (_allCardScrObjs[i].id != id) continue;
            return _allCardScrObjs[i];
        }
        return null;
    }
}
