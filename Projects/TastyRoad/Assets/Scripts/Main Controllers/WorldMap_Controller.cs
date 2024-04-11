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
        
    }



    //
    public void Set_RandomLocation(int worldNum)
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
        _mainController.Set_Location(worldNum, worldLocation[randLocationNum].locationNum);
    }
}
