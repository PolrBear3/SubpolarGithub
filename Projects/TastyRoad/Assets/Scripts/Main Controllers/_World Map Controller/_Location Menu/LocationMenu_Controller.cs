using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LocationMenu_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Vehicle_Controller _vehicle;


    [Header("")]
    [SerializeField] private Image _menuPanel;
    [SerializeField] private LocationTile[] _tiles;

    [Header("")]
    [SerializeField] private Image _cursor;
    [SerializeField] private UI_ClockTimer _holdClock;

    [SerializeField] private InformationBox _infoBox;
    
    
    [Space(80)]
    [SerializeField] private Input_Manager _inputManager;


    public Action<bool> On_MenuToggle;
    private int _hoverTileNum;


    // UnityEngine
    private void Start()
    {
        Toggle_Menu(false);
        
        // subscriptions
        _inputManager.OnCursorControl += CursorControl;
        _inputManager.OnHoldSelect += Select_HoverTile;
        _inputManager.OnExit += Exit;

        _inputManager.OnSelectStart += _holdClock.Run_ClockSprite;
        _inputManager.OnSelect += _holdClock.Stop_ClockSpriteRun;
        _inputManager.OnHoldSelect += _holdClock.Stop_ClockSpriteRun;
        
        Localization_Controller.instance.OnLanguageChanged += Update_InfoBox;
        Localization_Controller.instance.OnLanguageChanged += Update_NavigateText;
    }

    private void OnDestroy()
    {
        // subscriptions
        _inputManager.OnCursorControl -= CursorControl;
        _inputManager.OnHoldSelect -= Select_HoverTile;
        _inputManager.OnExit -= Exit;

        _inputManager.OnSelectStart -= _holdClock.Run_ClockSprite;
        _inputManager.OnSelect -= _holdClock.Stop_ClockSpriteRun;
        _inputManager.OnHoldSelect -= _holdClock.Stop_ClockSpriteRun;
        
        Localization_Controller.instance.OnLanguageChanged -= Update_InfoBox;
        Localization_Controller.instance.OnLanguageChanged -= Update_NavigateText;
    }


    // InputSystem
    private void CursorControl(Vector2 inputDirection)
    {
        if (inputDirection == Vector2.zero) return;
        if (Input_Controller.instance.isHolding) return;

        int xInput = (int)inputDirection.x;

        Update_HoverTileNum(xInput);

        Update_Cursor();
        Update_InfoBox();

        Update_TilesAnimation();
        
        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
    }

    private void Exit()
    {
        Toggle_Menu(false);
    }


    // Input_Controller
    public void Toggle_Menu(bool toggle)
    {
        Main_Controller main = Main_Controller.instance;

        main.Player().detection.Toggle_BoxCollider(!toggle);

        // menu toggle
        _menuPanel.gameObject.SetActive(toggle);
        _inputManager.Toggle_Input(toggle);

        On_MenuToggle?.Invoke(toggle);

        if (toggle == false) return;

        // update panel
        int currentWorldNum = main.worldMap.currentData.worldNum;
        Sprite panelSprite = main.dataController.World_Data(currentWorldNum).panelSprite;

        _menuPanel.sprite = panelSprite;

        _hoverTileNum = _tiles.Length / 2;

        Update_Cursor();

        Update_Tiles();
        Update_LockedTiles();
        Update_TilesAnimation();
        
        Update_InfoBox();
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

    private void Update_InfoBox()
    {
        int centerNum = _tiles.Length / 2;

        if (_hoverTileNum == centerNum)
        {
            _infoBox.gameObject.SetActive(false);
            return;
        }

        Main_Controller main = Main_Controller.instance;
        Data_Controller data = main.dataController;

        StationMenu_Controller stationMenu = main.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slotsController = stationMenu.controller.slotsController;

        int currentAmount = slotsController.StationAmount(stationMenu.currentDatas, data.Station_ScrObj("Oil Drum"));
        int requireAmount = Mathf.Abs(centerNum - _hoverTileNum);
        
        InfoTemplate_Trigger trigger = gameObject.GetComponent<InfoTemplate_Trigger>();
        Information_Template template = trigger.templates[0];
        
        template.Set_SmartInfo("currentAmount", currentAmount.ToString());
        template.Set_SmartInfo("requireAmount", requireAmount.ToString());
        
        _infoBox.Update_InfoText(trigger.TemplateString(0));
        
        _infoBox.gameObject.SetActive(true);
        _infoBox.Update_RectLayout();

        if (_hoverTileNum <= centerNum)
        {
            _infoBox.Flip_toDefault();
            return;
        }

        if (_infoBox.flipped) return;

        _infoBox.Flip();
    }


    private bool Select_Available()
    {
        if (_menuPanel.gameObject.activeSelf == false) return false;
        
        int centerNum = _tiles.Length / 2;

        if (_hoverTileNum == centerNum) return false;
        if (_tiles[_hoverTileNum].locked) return false;

        Main_Controller main = Main_Controller.instance;
        Data_Controller data = main.dataController;

        StationMenu_Controller stationMenu = main.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slotsController = stationMenu.controller.slotsController;

        int oilAmount = slotsController.StationAmount(stationMenu.currentDatas, data.Station_ScrObj(79025));
        int requireOil = Mathf.Abs(centerNum - _hoverTileNum);

        if (oilAmount < requireOil) return false;

        return true;
    }

    private void Select_HoverTile()
    {
        if (Select_Available() == false) return;

        Main_Controller main = Main_Controller.instance;
        Data_Controller data = main.dataController;

        WorldMap_Controller worldMap = main.worldMap;
        StationMenu_Controller stationMenu = main.currentVehicle.menu.stationMenu;

        int removeAmount = Mathf.Abs(_tiles.Length / 2 - _hoverTileNum);
        stationMenu.Remove_StationItem(data.Station_ScrObj(79025), removeAmount);

        worldMap.Update_Location(_tiles[_hoverTileNum].data);
        
        _inputManager.Toggle_Input(false);
        main.Player().detection.Toggle_BoxCollider(false);
    }


    // Tiles
    private void Update_Tiles()
    {
        Main_Controller main = Main_Controller.instance;
        Data_Controller data = main.dataController;

        WorldMap_Controller worldMap = main.worldMap;
        WorldMap_Data currentData = worldMap.currentData;

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
        Main_Controller main = Main_Controller.instance;
        Data_Controller data = main.dataController;

        WorldMap_Controller worldMap = main.worldMap;

        Update_PreviousTiles(data, worldMap);
        Update_NextTiles(data, worldMap);
    }

    private void Update_PreviousTiles(Data_Controller data, WorldMap_Controller worldMap)
    {
        int worldNum = worldMap.currentData.worldNum - 1;
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
        int worldNum = worldMap.currentData.worldNum + 1;
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
    
    
    // Other
    private void Update_NavigateText()
    {
        InfoTemplate_Trigger template = gameObject.GetComponent<InfoTemplate_Trigger>();
        template.setText.text = template.TemplateString(1);
    }
}