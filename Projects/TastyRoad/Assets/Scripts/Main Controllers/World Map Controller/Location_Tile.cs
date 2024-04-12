using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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



    // Data Control
    public void Update_WorldNum(int updateNum)
    {
        _worldNum += updateNum;

        // update tile sprite
    }



    // Animation Control
    public void Tile_UnPress()
    {
        _anim.Play("LocationTile_hover");
    }

    public void Tile_Press()
    {
        _anim.Play("LocationTile_press");
    }

    public void Tile_Hover()
    {
        _anim.Play("LocationTile_unpress");
    }
}
