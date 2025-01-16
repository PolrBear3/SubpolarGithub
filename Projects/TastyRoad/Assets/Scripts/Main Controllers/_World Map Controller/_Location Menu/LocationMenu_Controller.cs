using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LocationMenu_Controller : MonoBehaviour
{
    private PlayerInput _input;

    [Header("")]
    [SerializeField] private Vehicle_Controller _vehicle;


    [Header("")]
    [SerializeField] private Image _menuPanel;

    [Header("")]
    [SerializeField] private LocationTile[] _tiles;


    private int _hoverTileNum;


    // UnityEngine
    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();
    }

    private void Start()
    {
        Toggle_Menu(false);
    }


    // InputSystem
    private void OnCursorControl(InputValue value)
    {
        // if (_onHold) return;

        Vector2 input = value.Get<Vector2>();
        int xInput = (int)input.x;

        Update_HoverTile(xInput);
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
        int currentWorldNum = main.worldMap.currentWorldNum;
        Sprite panelSprite = main.dataController.World_Data(currentWorldNum).panelSprite;

        _menuPanel.sprite = panelSprite;

        // update tiles
        _hoverTileNum = 0;

        Update_Tiles();
        Update_HoverTile(0);
    }


    private void Update_HoverTile(int xInputDirection)
    {
        int unlockCount = Unlocked_TileCount();
        int previousNum = _hoverTileNum;

        _hoverTileNum += xInputDirection;
        _hoverTileNum = (_hoverTileNum + unlockCount) % unlockCount;

        _tiles[_hoverTileNum].anim.Play("LocationTile_hover");

        if (previousNum == 0)
        {
            _tiles[previousNum].anim.Play("LocationTile_press");
            return;
        }

        if (xInputDirection == 0) return;

        _tiles[previousNum].anim.Play("LocationTile_unpress");
    }

    private void Select_HoverTile()
    {
        if (_tiles[_hoverTileNum].locked) return;

        WorldMap_Controller worldMap = _vehicle.mainController.worldMap;
    }


    // Tiles
    private int Unlocked_TileCount()
    {
        Data_Controller data = _vehicle.mainController.dataController;
        WorldMap_Controller worldMap = _vehicle.mainController.worldMap;

        int currentLocationNum = worldMap.currentLocationNum;
        int totalLocationCount = data.LocationCount_inWorld(worldMap.currentWorldNum);

        int unlockCount = Mathf.Clamp(totalLocationCount - currentLocationNum, 0, _tiles.Length);

        return unlockCount;
    }

    private void Update_Tiles()
    {
        Data_Controller data = _vehicle.mainController.dataController;
        WorldMap_Controller worldMap = _vehicle.mainController.worldMap;

        int currentWorldNum = worldMap.currentWorldNum;
        int unlockCount = Unlocked_TileCount();

        for (int i = 0; i < _tiles.Length; i++)
        {
            if (unlockCount <= 0)
            {
                _tiles[i].Toggle_Lock(true);
                continue;
            }

            _tiles[i].Toggle_Lock(false);
            _tiles[i].Set_AnimatorOverrider(data.World_Data(currentWorldNum).tileAnimOverrider);

            unlockCount--;
        }
    }
}
