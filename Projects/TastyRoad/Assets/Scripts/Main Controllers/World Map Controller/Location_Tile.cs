using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location_Tile : MonoBehaviour
{
    private Animator _anim;

    [SerializeField] private Transform _cursorPoint;
    public Transform cursorPoint => _cursorPoint;

    private int _worldNum;
    public int worldNum => _worldNum;



    // UnityEngine
    private void Awake()
    {
        _anim = gameObject.GetComponent<Animator>();
    }



    //
    public void Tile_Select()
    {

    }

    public void Tile_Hover()
    {

    }
}
