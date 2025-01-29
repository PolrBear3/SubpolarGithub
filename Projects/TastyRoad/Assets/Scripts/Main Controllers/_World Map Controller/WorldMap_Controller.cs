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
    [SerializeField] private Main_Controller _mainController;


    public static Action OnNewLocation;

    private bool _newLocation;

    private WorldMap_Data _data;
    public WorldMap_Data data => _data;


    // UnityEngine
    private void Start()
    {
        // load saved location
        if (ES3.KeyExists("WorldMap_Controller/_data"))
        {
            Set_Location(_data);

            if (_newLocation == false) return;

            _newLocation = false;
            _mainController.currentLocation.Activate_NewSetEvents();

            return;
        }

        // new game location
        int randLocationNum = UnityEngine.Random.Range(0, _mainController.dataController.LocationCount_inWorld(0));
        Set_Location(new WorldMap_Data(0, randLocationNum));

        _newLocation = false;
        _mainController.currentLocation.Activate_NewSetEvents();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("WorldMap_Controller/_newLocation", _newLocation);
        ES3.Save("WorldMap_Controller/_data", _data);
    }

    public void Load_Data()
    {
        _newLocation = ES3.Load("WorldMap_Controller/_newLocation", _newLocation);
        _data = ES3.Load("WorldMap_Controller/_data", _data);
    }


    // Location Control
    private void Set_Location(WorldMap_Data setData)
    {
        _data = new(setData);

        // set location
        Location_Controller location = _mainController.Set_Location(_data);
        _mainController.Track_CurrentLocaiton(location);
    }

    public void Update_Location(WorldMap_Data updateData)
    {
        StartCoroutine(Update_Location_Coroutine(updateData));
    }
    private IEnumerator Update_Location_Coroutine(WorldMap_Data updateData)
    {
        Sprite worldIcon = _mainController.dataController.World_Data(updateData.worldNum).worldIcon;

        // transition curtain animation
        _mainController.transitionCanvas.Set_LoadIcon(worldIcon);
        _mainController.transitionCanvas.CloseScene_Transition();

        while (TransitionCanvas_Controller.transitionPlaying) yield return null;

        // reset settings before moving on to new location
        _mainController.Destroy_AllStations();
        _mainController.ResetAll_ClaimedPositions();

        // set new location
        Set_Location(updateData);

        // new location events
        _newLocation = true;
        OnNewLocation?.Invoke();

        SaveLoad_Controller.SaveAll_ISaveLoadable();
        Save_Data();

        // reload game scene
        SceneManager.LoadScene(0);
    }
}
