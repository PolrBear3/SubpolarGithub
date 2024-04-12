using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WorldMap_Controller : MonoBehaviour, ISaveLoadable
{
    private PlayerInput _input;

    private Main_Controller _mainController;

    [Header("")]
    [SerializeField] private RectTransform _backgroundPanel;
    [SerializeField] private GameObject _worldPanel;
    [SerializeField] private GameObject _cursor;

    [Header("")]
    [SerializeField] private Location_Tile[] _tiles;

    private int _currentTileNum;
    private int _cursorTileNum;



    // UnityEngine
    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();
        _mainController = FindObjectOfType<Main_Controller>();
    }

    private void Start()
    {
        // test
        Map_Toggle(true);

        if (ES3.KeyExists("Main_Controller/_currentLocationData"))
        {
            // set saved location
            Location_ScrObj loadScrObj = _mainController.savedLocationData.locationScrObj;

            Location_Controller location = _mainController.Set_Location(loadScrObj.worldNum, loadScrObj.locationNum);
            _mainController.Track_CurrentLocaiton(location);

            return;
        }

        // set new starting location
        _currentTileNum = _cursorTileNum;
        Set_RandomLocation(_tiles[_currentTileNum].worldNum);
    }



    // InputSystem
    private void OnCursorControl(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        int xInput = (int)input.x;

        Update_CursorTile(xInput);
    }

    private void OnSelect()
    {
        Select_CursorTile();
    }

    private void OnExit()
    {

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
            _tiles[_currentTileNum].Tile_Select();

            LeanTween.alpha(_backgroundPanel, 95 * 0.01f, 0.25f);
        }
        else
        {
            LeanTween.alpha(_backgroundPanel, 0f, 0.25f);
        }
    }

    private void Update_CursorTile(int cursorDirection)
    {
        _cursorTileNum += cursorDirection;

        if (_cursorTileNum < 0) _cursorTileNum = 0;
        else if (_cursorTileNum > _tiles.Length - 1) _cursorTileNum = _tiles.Length - 1;

        _cursor.transform.position = _tiles[_cursorTileNum].cursorPoint.position;

        if (_cursorTileNum == _currentTileNum) return;
        _tiles[_cursorTileNum].Tile_Hover();
    }

    private void Select_CursorTile()
    {
        // previous tile location
        Destroy(_mainController.currentLocation.gameObject);

        // set selected tile location
        _currentTileNum = _cursorTileNum;
        ES3.Save("WorldMap_Controller/_currentTileNum", _currentTileNum);

        Set_RandomLocation(_tiles[_currentTileNum].worldNum);

        // reload game scene
        SceneManager.LoadScene(0);
    }



    // Tile Control
    private void UpdateTile_WorldNum()
    {

    }



    //
    private void Set_RandomLocation(int worldNum)
    {
        List<Location_ScrObj> allLocations = _mainController.dataController.locations;
        List<Location_ScrObj> worldLocation = new();

        // get locations that are worldNum
        for (int i = 0; i < allLocations.Count; i++)
        {
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
}
