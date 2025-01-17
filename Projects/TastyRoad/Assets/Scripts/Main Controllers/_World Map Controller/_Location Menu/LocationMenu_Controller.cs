using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

public class LocationMenu_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private PlayerInput _input;
    [SerializeField] private HoldInput_Controller _holdInput;

    [Header("")]
    [SerializeField] private Vehicle_Controller _vehicle;


    [Header("")]
    [SerializeField] private Image _menuPanel;

    [Header("")]
    [SerializeField] private LocationTile[] _tiles;
    [SerializeField] private Image _cursor;


    private int _hoverTileNum;


    // UnityEngine
    private void Start()
    {
        Toggle_Menu(false);

        // subscriptions
        _holdInput.OnHoldComplete += Select_HoverTile;
    }

    private void OnDestroy()
    {
        // subscriptions
        _holdInput.OnHoldComplete -= Select_HoverTile;
    }


    // InputSystem
    private void OnCursorControl(InputValue value)
    {
        // if (_onHold) return;

        Vector2 input = value.Get<Vector2>();
        int xInput = (int)input.x;

        Update_HoverTileNum(xInput);
        Update_Cursor();

        Update_TilesAnimation();
    }

    private void OnExit()
    {
        Toggle_Menu(false);
    }


    // Controls
    public void Toggle_Menu(bool toggle)
    {
        Main_Controller main = _vehicle.mainController;

        // disable player controls
        main.Player().Toggle_Controllers(!toggle);

        // menu toggle
        _menuPanel.gameObject.SetActive(toggle);
        _input.enabled = toggle;

        if (toggle == false) return;

        // update panel
        int currentWorldNum = main.worldMap.data.worldNum;
        Sprite panelSprite = main.dataController.World_Data(currentWorldNum).panelSprite;

        _menuPanel.sprite = panelSprite;

        _hoverTileNum = _tiles.Length / 2;
        Update_Cursor();

        Update_Tiles();
        Update_LockedTiles();
        Update_TilesAnimation();
    }


    private void Update_HoverTileNum(int xInputDirection)
    {
        int tileCount = _tiles.Length;
        int prevHoverNum = _hoverTileNum;

        _hoverTileNum = (_hoverTileNum + xInputDirection + tileCount) % tileCount;

        for (int i = 0; i < tileCount; i++)
        {
            int newHoverNum = (_hoverTileNum + i * xInputDirection + tileCount) % tileCount;

            if (_tiles[newHoverNum].locked) continue;

            _hoverTileNum = newHoverNum;
            return;
        }

        _hoverTileNum = prevHoverNum;
    }

    private void Update_Cursor()
    {
        _cursor.transform.SetParent(_tiles[_hoverTileNum].cursorPoint);
        _cursor.rectTransform.localPosition = Vector2.zero;
    }


    private void Select_HoverTile()
    {
        if (_hoverTileNum == _tiles.Length / 2) return;
        if (_tiles[_hoverTileNum].locked) return;

        WorldMap_Controller worldMap = _vehicle.mainController.worldMap;

        _input.enabled = false;
        worldMap.Update_Location(_tiles[_hoverTileNum].data);
    }


    // Tiles
    private void Update_Tiles()
    {
        Data_Controller data = _vehicle.mainController.dataController;
        WorldMap_Controller worldMap = _vehicle.mainController.worldMap;

        WorldMap_Data currentData = worldMap.data;

        int worldNum = currentData.worldNum;
        int locationNum = currentData.locationNum;

        int totalLocationCount = data.LocationCount_inWorld(worldNum);

        for (int i = 0; i < _tiles.Length; i++)
        {
            // location number relative to the middle tile
            int relativeLocationNum = locationNum - _tiles.Length / 2 + i;

            if (relativeLocationNum < 0 || relativeLocationNum >= totalLocationCount)
            {
                _tiles[i].Toggle_Lock(true);
                continue;
            }

            _tiles[i].Toggle_Lock(false);
            _tiles[i].Set_WorldMapData(new(worldNum, relativeLocationNum));
            _tiles[i].Set_AnimatorOverrider(data.World_Data(worldNum).tileAnimOverrider);
        }
    }

    private void Update_TilesAnimation()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (i == _tiles.Length / 2)
            {
                _tiles[i].anim.Play("LocationTile_press");
                continue;
            }

            if (i == _hoverTileNum)
            {
                _tiles[i].anim.Play("LocationTile_hover");
                continue;
            }

            _tiles[i].anim.Play("LocationTile_unpress");
        }
    }


    private void Update_LockedTiles()
    {
        Data_Controller data = _vehicle.mainController.dataController;
        WorldMap_Controller worldMap = _vehicle.mainController.worldMap;

        Update_PreviousTiles(data, worldMap);
        Update_NextTiles(data, worldMap);
    }

    private void Update_PreviousTiles(Data_Controller data, WorldMap_Controller worldMap)
    {
        int worldNum = worldMap.data.worldNum - 1;
        int locationNum = data.LocationCount_inWorld(worldNum);

        for (int i = _tiles.Length / 2 - 1; i >= 0; i--)
        {
            if (!_tiles[i].locked) continue;

            locationNum--;

            if (locationNum < 0)
            {
                worldNum--;
                locationNum = data.LocationCount_inWorld(worldNum) - 1;

                if (locationNum < 0) break;
            }

            _tiles[i].Toggle_Lock(false);
            _tiles[i].Set_WorldMapData(new(worldNum, locationNum));
            _tiles[i].Set_AnimatorOverrider(data.World_Data(worldNum).tileAnimOverrider);
        }
    }
    private void Update_NextTiles(Data_Controller data, WorldMap_Controller worldMap)
    {
        int worldNum = worldMap.data.worldNum + 1;
        int locationNum = 0;

        for (int i = _tiles.Length / 2 + 1; i < _tiles.Length; i++)
        {
            if (!_tiles[i].locked) continue;

            if (locationNum >= data.LocationCount_inWorld(worldNum))
            {
                worldNum++;
                locationNum = 0;

                if (worldNum >= data.worldData.Length) break;
            }

            _tiles[i].Toggle_Lock(false);
            _tiles[i].Set_WorldMapData(new(worldNum, locationNum));
            _tiles[i].Set_AnimatorOverrider(data.World_Data(worldNum).tileAnimOverrider);

            locationNum++;
        }
    }
}