using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPlacer : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private GameObject _switchPrefab;
    
    
    private Switch_SrcObj _placingSwitch;
    public Switch_SrcObj placingSwitch => _placingSwitch;

    
    // MonoBehaviour
    private void Start()
    {
        TileBoard tileBoard = Main_Controller.instance.tileBoard;

        tileBoard.OnTileHover += Preview_PlacingSwitch;
        tileBoard.OnTileSelect += Place_Switch;
    }

    private void OnDestroy()
    {
        TileBoard tileBoard = Main_Controller.instance.tileBoard;

        tileBoard.OnTileHover -= Preview_PlacingSwitch;
        tileBoard.OnTileSelect -= Place_Switch;
    }


    // Data
    public void Set_PlacingSwitch(Switch_SrcObj placingSwitch)
    {
        _placingSwitch = placingSwitch;
    }
    
    
    // Main
    private void Preview_PlacingSwitch()
    {
        if (_placingSwitch == null) return;
        
        TileBoard tileBoard = Main_Controller.instance.tileBoard;
        
        tileBoard.previousTile.Toggle_HoverIndication(false);
        tileBoard.hoverTile.Toggle_HoverIndication(true);
    }

    private void ResetAll_PlacingPreviews()
    {
        List<Tile> allTiles = Main_Controller.instance.tileBoard.setTiles;

        for (int i = 0; i < allTiles.Count; i++)
        {
            allTiles[i].Toggle_HoverIndication(false);
        }
    }
    
    
    private void Place_Switch(Tile placeTile)
    {
        if (_placingSwitch == null) return;
        
        GameObject switchObject = Instantiate(_switchPrefab, placeTile.transform.position, Quaternion.identity);
        switchObject.transform.SetParent(placeTile.transform);
        
        // set switch data
        Switch placedSwitch = switchObject.GetComponent<Switch>();
        
        placedSwitch.Set_Data(new(_placingSwitch));
        placedSwitch.Load_DataIndication();
        
        Inventory inventory = Main_Controller.instance.inventory;
        Cursor cursor = inventory.cursor;
        
        cursor.image.color = Color.clear;
        cursor.Toggle_Activation(false);
        
        ResetAll_PlacingPreviews();
        
        _placingSwitch = null;
        inventory.cursor.OnLayerClick -= Cancel_PlacingSwitch;
    }

    public void Cancel_PlacingSwitch(int clickedLayerIndex)
    {
        int tileLayerIndex = Main_Controller.instance.tileBoard.tileLayerIndex;
        if (clickedLayerIndex == tileLayerIndex) return;
        
        Inventory inventory = Main_Controller.instance.inventory;
        inventory.Add_Switch(_placingSwitch);
        
        Cursor cursor = inventory.cursor;
        
        cursor.image.color = Color.clear;
        cursor.Toggle_Activation(false);

        ResetAll_PlacingPreviews();
        
        _placingSwitch = null;
        inventory.cursor.OnLayerClick -= Cancel_PlacingSwitch;
    }
}
