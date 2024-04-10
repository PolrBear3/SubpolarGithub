using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap_Controller : MonoBehaviour
{
    private Main_Controller _mainController;

    [SerializeField] private GameObject _cursor;



    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
    }

    private void Start()
    {
        Set_Location(0);
    }



    //
    private void Set_Location(int worldNum)
    {
        List<GameObject> allLocations = _mainController.dataController.locations;

        // get locations that are worldNum
        List<GameObject> worldNumLocations = new();

        for (int i = 0; i < allLocations.Count; i++)
        {
            Location_Controller controller = allLocations[i].GetComponent<Location_Controller>();

            // differect location from current location
            if (_mainController.currentLocation == controller) continue;
            // same world num
            if (worldNum != controller.currentData.worldNum) continue;

            worldNumLocations.Add(allLocations[i]);
        }

        // set random location num
        int randArrayNum = Random.Range(0, worldNumLocations.Count);

        // set location
        _mainController.Set_Location(worldNumLocations[randArrayNum]);
    }
}
