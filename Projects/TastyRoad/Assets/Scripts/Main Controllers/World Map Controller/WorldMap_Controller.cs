using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WorldMap_Controller : MonoBehaviour, ISaveLoadable
{
    private PlayerInput _input;

    private Main_Controller _mainController;

    [Header("")]
    [SerializeField] private RectTransform _backgroundPanel;
    [SerializeField] private GameObject _worldPanel;

    [Header("")]
    [SerializeField] private Image _cursorImage;
    [SerializeField] private UI_ClockTimer _holdTimer;

    [Header("")]
    [SerializeField] private Location_Tile[] _tiles;

    private int _currentTileNum;
    private int _cursorTileNum;



    // UnityEngine
    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }

    private void Start()
    {
        if (ES3.KeyExists("Main_Controller/_currentLocationData"))
        {
            // set previous saved location
            Location_ScrObj loadScrObj = _mainController.savedLocationData.locationScrObj;

            Location_Controller location = _mainController.Set_Location(loadScrObj.worldNum, loadScrObj.locationNum);
            _mainController.Track_CurrentLocaiton(location);

            return;
        }

        // set new game starting location
        _currentTileNum = _cursorTileNum;

        Set_RandomLocation();
        _mainController.currentLocation.Activate_LocationSet_Events();
    }



    // InputSystem
    private void OnCursorControl(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        int xInput = (int)input.x;

        Update_CursorTile(xInput);
        UpdateTiles_Animation();
    }

    private void OnSelect()
    {
        _cursorImage.color = Color.white;
        _holdTimer.Stop_ClockSpriteRun();
    }

    private void OnSelectDown()
    {
        _cursorImage.color = Color.clear;
        _holdTimer.Run_ClockSprite();
    }

    private void OnHoldSelect()
    {
        Select_CursorTile();
    }

    private void OnExit()
    {
        Map_Toggle(false);
    }



    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("WorldMap_Controller/_currentTileNum", _currentTileNum);
    }

    public void Load_Data()
    {
        _currentTileNum = ES3.Load("WorldMap_Controller/_currentTileNum", _currentTileNum);
    }



    // Menu Control
    public void Map_Toggle(bool toggleOn)
    {
        Main_Controller.gamePaused = toggleOn;

        _worldPanel.SetActive(toggleOn);
        _input.enabled = toggleOn;

        if (toggleOn)
        {
            Update_CursorTile(_currentTileNum);
            UpdateTiles_Animation();

            LeanTween.alpha(_backgroundPanel, 95 * 0.01f, 0.25f);
        }
        else
        {
            _cursorImage.color = Color.white;
            _holdTimer.Stop_ClockSpriteRun();

            LeanTween.alpha(_backgroundPanel, 0f, 0.25f);
        }
    }



    // Single Tile to Tile Control
    private void Update_CursorTile(int cursorDirection)
    {
        // cursor postion
        _cursorTileNum += cursorDirection;

        if (_cursorTileNum < 0) _cursorTileNum = 0;
        else if (_cursorTileNum > _tiles.Length - 1) _cursorTileNum = _tiles.Length - 1;

        _cursorImage.transform.position = _tiles[_cursorTileNum].cursorPoint.position;
    }

    private void Select_CursorTile()
    {
        // check if new location tile is selected
        if (_currentTileNum == _cursorTileNum)
        {
            Map_Toggle(false);
            return;
        }

        // reset settings before moving on to new location
        _mainController.Destroy_AllStations();
        _mainController.Destroy_AllCharacters();
        _mainController.ResetAll_ClaimedPositions();

        // set selected tile location
        _currentTileNum = _cursorTileNum;

        // set new location
        Set_RandomLocation(_tiles[_currentTileNum].worldNum);
        _mainController.currentLocation.Activate_LocationSet_Events();

        // reload game scene
        Main_Controller.gamePaused = false;
        SaveLoad_Controller.SaveAll_ISaveLoadable();
        SceneManager.LoadScene(0);
    }



    // All Tile Control
    private void UpdateTiles_WorldNum(int updateNum)
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            _tiles[i].Update_WorldNum(updateNum);
        }
    }

    private void UpdateTiles_Animation()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (i == _currentTileNum)
            {
                _tiles[i].Tile_Press();
                _tiles[i].gasCoinIndicator.SetActive(false);
                _tiles[i].currentTileIndicator.SetActive(true);

                continue;
            }

            if (i == _cursorTileNum)
            {
                _tiles[i].Tile_Hover();
                _tiles[i].GasCoin_ToggleOn(Mathf.Abs(_cursorTileNum - _currentTileNum));
                _tiles[i].currentTileIndicator.SetActive(false);

                continue;
            }

            _tiles[i].Tile_UnPress();
            _tiles[i].gasCoinIndicator.SetActive(false);
            _tiles[i].currentTileIndicator.SetActive(false);
        }
    }




    /// <summary>
    /// Random location setting for new starting game
    /// </summary>
    private void Set_RandomLocation()
    {
        List<Location_ScrObj> allLocations = _mainController.dataController.locations;
        List<Location_ScrObj> worldLocation = new();

        int worldNum = _tiles[_currentTileNum].worldNum;

        // get locations that are worldNum
        for (int i = 0; i < allLocations.Count; i++)
        {
            // check if same world
            if (worldNum != allLocations[i].worldNum) continue;

            worldLocation.Add(allLocations[i]);
        }

        // set random location num
        int randLocationNum = Random.Range(0, worldLocation.Count);

        // set location
        Location_Controller location = _mainController.Set_Location(worldNum, worldLocation[randLocationNum].locationNum);

        // track location
        _mainController.Track_CurrentLocaiton(location);
    }

    /// <summary>
    /// Random location setting for selecting location tile
    /// </summary>
    private void Set_RandomLocation(int worldNum)
    {
        List<Location_ScrObj> allLocations = _mainController.dataController.locations;
        List<Location_ScrObj> worldLocation = new();

        // get locations that are worldNum
        for (int i = 0; i < allLocations.Count; i++)
        {
            // check if same world
            if (worldNum != allLocations[i].worldNum) continue;

            // check if different location
            if (_mainController.currentLocation.setData.locationScrObj == allLocations[i]) continue;

            worldLocation.Add(allLocations[i]);
        }

        // set random location num
        int randLocationNum = Random.Range(0, worldLocation.Count);

        // set location
        Location_Controller location = _mainController.Set_Location(worldNum, worldLocation[randLocationNum].locationNum);

        // track location
        _mainController.Track_CurrentLocaiton(location);
    }
}
