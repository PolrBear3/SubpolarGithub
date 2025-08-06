using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMOD.Studio;

[System.Serializable]
public struct WorldMap_Data
{
    [ES3Serializable] private int _worldNum;
    public int worldNum => _worldNum;

    [ES3Serializable] private int _locationNum;
    public int locationNum => _locationNum;


    // New
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
    [Space(20)]
    [SerializeField] private Location_ScrObj _startingLocation;


    /// <summary>
    /// Invokes before scene reload
    /// </summary>
    public Action OnNewLocation;
    public Action OnLocationSet;

    private bool _newLocation;

    private WorldMap_Data _currentData;
    public WorldMap_Data currentData => _currentData;


    private HashSet<WorldMap_Data> _visitedDatas = new();
    public HashSet<WorldMap_Data> visitedDatas => _visitedDatas;


    // UnityEngine
    private void Start()
    {
        Location_Controller location = null;
        
        // load saved location
        if (ES3.KeyExists("WorldMap_Controller/WorldMap_Data"))
        {
            location = Set_Location(_currentData);
            Play_LocationBGM(location);

            if (_newLocation == false) return;

            Activate_NewLocationEvents();
            return;
        }

        // new game location
        WorldMap_Data newLocation = new(_startingLocation.worldNum, _startingLocation.locationNum);
        
        location = Set_Location(newLocation);
        Play_LocationBGM(location);

        Activate_NewLocationEvents();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("WorldMap_Controller/_newLocation", _newLocation);

        ES3.Save("WorldMap_Controller/WorldMap_Data", _currentData);
        ES3.Save("WorldMap_Controller/_visitedDatas", _visitedDatas);
    }

    public void Load_Data()
    {
        _newLocation = ES3.Load("WorldMap_Controller/_newLocation", _newLocation);

        _currentData = ES3.Load("WorldMap_Controller/WorldMap_Data", _currentData);
        _visitedDatas = ES3.Load("WorldMap_Controller/_visitedDatas", _visitedDatas);
    }


    // Location Control
    private Location_Controller Set_Location(WorldMap_Data setData)
    {
        _currentData = new(setData);

        // set location
        Main_Controller main = Main_Controller.instance;
        Location_Controller location = main.Set_Location(_currentData);

        main.Track_CurrentLocaiton(location);
        OnLocationSet?.Invoke();

        return location;
    }

    public void Update_Location(WorldMap_Data updateData)
    {
        StartCoroutine(Update_Location_Coroutine(updateData));
    }
    private IEnumerator Update_Location_Coroutine(WorldMap_Data updateData)
    {
        Main_Controller main = Main_Controller.instance;
        Sprite worldIcon = main.dataController.World_Data(updateData.worldNum).worldIcon;
        
        main.Player().Toggle_Controllers(false);
        
        // bgm
        Main_Controller.instance.currentLocation.GetComponent<SoundData_Controller>().FadeOut(0);

        // transition curtain animation
        TransitionCanvas_Controller transition = TransitionCanvas_Controller.instance;
        
        transition.Set_LoadIcon(worldIcon);
        transition.CloseScene_Transition();

        while (transition.coroutine != null) yield return null;

        // reset settings before moving on to new location
        main.Destroy_AllStations();
        main.ResetAll_ClaimedPositions();

        // set new location
        Set_Location(updateData);

        // new events before save & reload
        _newLocation = true;
        OnNewLocation?.Invoke();

        // save current activated events
        SaveLoad_Controller.instance.SaveAll_ISaveLoadable();
        Save_Data();

        // reload game scene
        SceneManager.LoadScene(1);
    }


    private void Activate_NewLocationEvents()
    {
        Location_Controller currentLocation = Main_Controller.instance.currentLocation;

        _newLocation = false;

        if (!_visitedDatas.Contains(_currentData))
        {
            _visitedDatas.Add(_currentData);
            currentLocation.OnFirstVisit?.Invoke();

            return;
        }
        currentLocation.OnNewLocationSet?.Invoke();
    }


    private void Play_LocationBGM(Location_Controller location)
    {
        EventInstance eventInstance = Audio_Controller.instance.Create_EventInstance(location.gameObject, 0);
        eventInstance.setParameterByName("Value_intensity", 0f);
        eventInstance.start();
        
        location.gameObject.GetComponent<SoundData_Controller>().FadeIn(0);
    }
}
