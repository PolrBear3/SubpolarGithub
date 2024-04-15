using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapSpawner : MonoBehaviour
{
    private Main_Controller _mainController;

    [Header("")]
    [SerializeField] private int _spawnAmount;


    // UnityEngine
    private void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();

        WorldMap_Controller.NewLocation_Event += Spawn_Scraps;
    }

    private void OnDestroy()
    {
        WorldMap_Controller.NewLocation_Event -= Spawn_Scraps;
    }



    //
    private void Spawn_Scraps()
    {
        LocationData d = _mainController.currentLocation.currentData;

        for (int i = 0; i < _spawnAmount; i++)
        {
            int randX = Random.Range((int)d.spawnRangeX.x, (int)d.spawnRangeX.y + 1);
            int randY = Random.Range((int)d.spawnRangeY.x, (int)d.spawnRangeY.y + 1);

            Vector2 spawnPos = new(randX, randY);

            if (_mainController.Position_Claimed(spawnPos))
            {
                i--;
                continue;
            }

            Station_ScrObj scrapScrObj = _mainController.dataController.Station_ScrObj("Scrap");
            Station_Controller scrap = _mainController.Spawn_Station(scrapScrObj, spawnPos);
            scrap.movement.Load_Position();
        }
    }
}