using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private Station_Controller _stationController;
    public Station_Controller stationController => _stationController;

    [SerializeField] private ActionBubble_Interactable _interactable;

    [Header("")]
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


    //
    private void Set_RandomSprite()
    {
        int randArrayNum = Random.Range(0, _scrapSprites.Count);
        _sr.sprite = _scrapSprites[randArrayNum];
    }
}
