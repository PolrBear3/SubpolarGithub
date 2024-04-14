using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Location_Tile : MonoBehaviour
{
    private Animator _anim;

    [Header("")]
    [SerializeField] private Transform _cursorPoint;
    public Transform cursorPoint => _cursorPoint;

    [Header("")]
    [SerializeField] private GameObject _currentTileIndicator;
    public GameObject currentTileIndicator => _currentTileIndicator;

    [Header("")]
    [SerializeField] private GameObject _gasCoinIndicator;
    public GameObject gasCoinIndicator => _gasCoinIndicator;

    [SerializeField] private TextMeshProUGUI _gasCoinAmount;

    private int _worldNum;
    public int worldNum => _worldNum;



    // UnityEngine
    private void Awake()
    {
        _anim = gameObject.GetComponent<Animator>();
    }



    // Data Control
    public void Update_WorldNum(int updateNum)
    {
        _worldNum += updateNum;

        // update tile sprite according to world type
    }



    // Animation Control
    public void Tile_UnPress()
    {
        _anim.Play("LocationTile_unpress");
    }

    public void Tile_Press()
    {
        _anim.Play("LocationTile_press");
    }

    public void Tile_Hover()
    {
        _anim.Play("LocationTile_hover");
    }



    // Gas Coin Indicator Control
    public void GasCoin_ToggleOn(int coinAmount)
    {
        _gasCoinAmount.text = coinAmount.ToString();
        _gasCoinIndicator.SetActive(true);
    }
}
