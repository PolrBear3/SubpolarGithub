using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location_Controller : MonoBehaviour
{
    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;

    [SerializeField] private LocationData _setData;
    public LocationData setData => _setData;

    private LocationData _currentData;
    public LocationData currentData => _currentData;

    [Header("")]
    [SerializeField] private SpriteRenderer _roamArea;
    public SpriteRenderer roamArea => _roamArea;

    private MaxSpawn_TimePoint _currentTimePoint;



    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();

        SetCurrent_LocationData();
    }

    private void Start()
    {
        // Toggle Off All Roam Area Colors On Game Start
        _roamArea.color = Color.clear;

        Update_Current_MaxSpawn();
        GlobalTime_Controller.TimeTik_Update += Update_Current_MaxSpawn;

        NPC_Spawn_Control();
    }



    // Current Data Control
    private void SetCurrent_LocationData()
    {
        _currentData = _setData;
    }



    /// <returns>
    /// True if checkPosition is in restricted range, False if not
    /// </returns>
    public bool Restricted_Position(Vector2 checkPosition)
    {
        LocationData d = _currentData;

        bool restricted = false;

        float xValue = checkPosition.x;
        if (xValue < d.spawnRangeX.x || xValue > d.spawnRangeX.y) restricted = true;

        float yValue = checkPosition.y;
        if (yValue < d.spawnRangeY.x || yValue > d.spawnRangeY.y) restricted = true;

        return restricted;
    }



    /// <summary>
    /// Also updates current npc population (global time tik update event)
    /// </summary>
    private void Update_Current_MaxSpawn()
    {
        LocationData d = _currentData;

        for (int i = 0; i < d.maxSpawnTimePoints.Count; i++)
        {
            if (_mainController.globalTime.currentTime != d.maxSpawnTimePoints[i].timePoint) continue;

            _currentTimePoint = d.maxSpawnTimePoints[i];
            Update_NPC_Population();

            return;
        }
    }
    //
    private void Update_NPC_Population()
    {
        // get current npc and amount
        List<GameObject> currentCharacters = _mainController.currentCharacters;
        List<NPC_Controller> currentNPCs = new();

        for (int i = 0; i < currentCharacters.Count; i++)
        {
            if (currentCharacters[i] == null) continue;
            if (!currentCharacters[i].TryGetComponent(out NPC_Controller npc)) continue;
            currentNPCs.Add(npc);
        }

        // if current npc amount is more than _currentMaxSpawn
        int npcOverFlowCount = currentCharacters.Count - _currentTimePoint.maxSpawnAmount;
        if (npcOverFlowCount <= 0) return;

        // current location roaming npc only leave
        for (int i = 0; i < currentNPCs.Count; i++)
        {
            NPC_Movement move = currentNPCs[i].movement;

            if (move.currentRoamArea != _roamArea) continue;
            if (move.isLeaving) continue;

            move.Leave(0f);

            npcOverFlowCount--;
            if (npcOverFlowCount <= 0) break;
        }
    }



    /// <summary>
    /// Update function for npc spawn and amount control
    /// </summary>
    public void NPC_Spawn_Control()
    {
        StartCoroutine(NPC_Spawn_Control_Coroutine());
    }
    private IEnumerator NPC_Spawn_Control_Coroutine()
    {
        LocationData d = _currentData;

        while (true)
        {
            while (_mainController.currentCharacters.Count >= _currentTimePoint.maxSpawnAmount) yield return null;

            float randIntervalTime = Random.Range(d.spawnIntervalTimeRange.x, d.spawnIntervalTimeRange.y);
            yield return new WaitForSeconds(randIntervalTime);

            _mainController.Spawn_Character(1, _mainController.OuterCamera_Position(Random.Range(0, 2)));
        }
    }



    // Scrap
    private void Spawn_Scraps(int amount)
    {
        LocationData d = _currentData;

        for (int i = 0; i < amount; i++)
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