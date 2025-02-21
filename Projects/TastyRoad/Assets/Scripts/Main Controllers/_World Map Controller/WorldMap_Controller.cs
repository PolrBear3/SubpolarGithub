using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct WorldMap_Data
{
    [ES3Serializable] private int _worldNum;
    public int worldNum => _worldNum;

    [ES3Serializable] private int _locationNum;
    public int locationNum => _locationNum;


    public WorldMap_Data(WorldMap_Data data)
    {
        _worldNum = data.worldNum;
        _locationNum = data.locationNum;
    }

    public WorldMap_Data(int worldNum, int locationNum)
    {
        _worldNum = worldNum;
        _locationNum = locationNum;
    }
}

public class WorldMap_Controller : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private Location_ScrObj _startingLocation;


    public static Action OnNewLocation;

    private bool _newLocation;

    private WorldMap_Data _currentData;
    public WorldMap_Data currentData => _currentData;


    private HashSet<WorldMap_Data> _visitedDatas = new();
    public HashSet<WorldMap_Data> visitedDatas => _visitedDatas;


    // UnityEngine
    private void Start()
    {
        // load saved location
        if (ES3.KeyExists("WorldMap_Controller/_currentData"))
        {
            Set_Location(_currentData);

            if (_newLocation == false) return;

            Activate_NewLocationEvents();
            return;
        }

        // new game location
        WorldMap_Data newLocation = new(_startingLocation.worldNum, _startingLocation.locationNum);
        Set_Location(newLocation);

        Activate_NewLocationEvents();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("WorldMap_Controller/_newLocation", _newLocation);

        ES3.Save("WorldMap_Controller/_currentData", _currentData);
        ES3.Save("WorldMap_Controller/_visitedDatas", _visitedDatas);
    }

    public void Load_Data()
    {
        _newLocation = ES3.Load("WorldMap_Controller/_newLocation", _newLocation);

        _currentData = ES3.Load("WorldMap_Controller/_currentData", _currentData);
        _visitedDatas = ES3.Load("WorldMap_Controller/_visitedDatas", _visitedDatas);
    }


    // Location Control
    private void Set_Location(WorldMap_Data setData)
    {
        _currentData = new(setData);

        // set location
        Main_Controller main = Main_Controller.instance;
        Location_Controller location = main.Set_Location(_currentData);

        main.Track_CurrentLocaiton(location);
    }

    public void Update_Location(WorldMap_Data updateData)
    {
        StartCoroutine(Update_Location_Coroutine(updateData));
    }
    private IEnumerator Update_Location_Coroutine(WorldMap_Data updateData)
    {
        Main_Controller main = Main_Controller.instance;
        Sprite worldIcon = main.dataController.World_Data(updateData.worldNum).worldIcon;

        // transition curtain animation
        main.transitionCanvas.Set_LoadIcon(worldIcon);
        main.transitionCanvas.CloseScene_Transition();

        while (TransitionCanvas_Controller.transitionPlaying) yield return null;

        // reset settings before moving on to new location
        main.Destroy_AllStations();
        main.ResetAll_ClaimedPositions();

        // set new location
        Set_Location(updateData);

        // new events before save & reload
        _newLocation = true;
        OnNewLocation?.Invoke();

        // save current activated events
        SaveLoad_Controller.SaveAll_ISaveLoadable();
        Save_Data();

        // reload game scene
        SceneManager.LoadScene(0);
    }


    private void Activate_NewLocationEvents()
    {
        Location_Controller currentLocation = Main_Controller.instance.currentLocation;

        _newLocation = false;

        if (!_visitedDatas.Contains(_currentData))
        {
            _visitedDatas.Add(_currentData);
            currentLocation.OnFirstVisit?.Invoke();
        }

        currentLocation.OnNewLocationSet?.Invoke();
    }
}
