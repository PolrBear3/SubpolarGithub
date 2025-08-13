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

public class WorldMap_Controller : MonoBehaviour, ISaveLoadable, IBackupLoadable
{
    [Space(20)]
    [SerializeField] private Location_ScrObj _startingLocation;


    private WorldMap_ControllerData _data;
    public WorldMap_ControllerData data => _data;
    
    /// <summary>
    /// Invokes before scene reload
    /// </summary>
    public Action OnNewLocation;
    public Action OnLocationSet;


    // UnityEngine
    private void Start()
    {
        Location_Controller location = Load_CurrentLocation();
        Play_LocationBGM(location);
        
        // load saved location
        if (ES3.KeyExists("WorldMap_Controller/WorldMap_ControllerData"))
        {
            if (_data.newLocation == false) return;

            Activate_NewLocationEvents();
            return;
        }

        // new game location
        Activate_NewLocationEvents();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("WorldMap_Controller/WorldMap_ControllerData", _data);
    }

    public void Load_Data()
    {
        if (ES3.KeyExists("WorldMap_Controller/WorldMap_ControllerData"))
        {
            _data = ES3.Load<WorldMap_ControllerData>("WorldMap_Controller/WorldMap_ControllerData");
            return;
        }
        
        WorldMap_Data startLocationData = new(_startingLocation.worldNum, _startingLocation.locationNum);
        _data = ES3.Load("WorldMap_Controller/WorldMap_ControllerData", new WorldMap_ControllerData(startLocationData));
    }
    
    
    // IBackupLoadable
    public bool Has_Conflict()
    {
        return false;
    }

    public void Load_Backup()
    {
        Debug.Log("WorldMap_Controller.Load_Backup");
    }


    // Location Control
    private Location_Controller Load_CurrentLocation()
    {
        Main_Controller main = Main_Controller.instance;
        Location_Controller location = main.Set_Location(_data.currentData);

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
        main.data.claimedPositions.Clear();

        // set new location
        _data.Set_WorldMapData(updateData);
        Load_CurrentLocation();

        // new events before save & reload
        _data.Toggle_NewLocation(true);
        OnNewLocation?.Invoke();

        // save current activated events
        SaveLoad_Controller.instance.SaveAll_ISaveLoadable();
        Save_Data();

        // reload game scene
        SceneManager.LoadScene(1);
    }
    
    private void Activate_NewLocationEvents()
    {
        _data.Toggle_NewLocation(false);
        
        Location_Controller currentLocation = Main_Controller.instance.currentLocation;

        if (_data.CurrentLocation_Visited() == false)
        {
            _data.visitedDatas.Add(_data.currentData);
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
