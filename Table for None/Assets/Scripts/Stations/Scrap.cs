using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    private SpriteRenderer _sr;

    
    [Space(20)]
    [SerializeField] private Station_Controller _stationController;
    public Station_Controller stationController => _stationController;

    [Space(20)]
    [SerializeField] private List<Sprite> _scrapSprites = new();
    

    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _stationController = gameObject.GetComponent<Station_Controller>();
    }

    private void Start()
    {
        Set_RandomSprite();
    }


    // Main
    private void Set_RandomSprite()
    {
        int randArrayNum = UnityEngine.Random.Range(0, _scrapSprites.Count);
        _sr.sprite = _scrapSprites[randArrayNum];
    }
}
