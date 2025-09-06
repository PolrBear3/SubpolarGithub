using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    
    [Space(20)]
    [SerializeField] private Clickable_EventSystem _eventSystem;


    private Tile_Data _data;
    public Tile_Data data => _data;


    // MonoBehaviour
    private void Start()
    {
        _eventSystem.OnEnter += Update_HoverTile;
        _eventSystem.OnClick += Select;
    }

    private void OnDestroy()
    {
        _eventSystem.OnEnter -= Update_HoverTile;
        _eventSystem.OnClick -= Select;
    }


    // Main
    private void Update_HoverTile()
    {
        Main_Controller.instance.tileBoard.Update_HoverTile(this);
    }
    
    public void Select()
    {
        Main_Controller.instance.tileBoard.OnTileSelect?.Invoke(this);
    }
    
    
    // Indication
    public void Toggle_HoverIndication(bool toggle)
    {
        Color color = toggle ? Color.gray : Color.white;
        _sr.color = color;
    }
}