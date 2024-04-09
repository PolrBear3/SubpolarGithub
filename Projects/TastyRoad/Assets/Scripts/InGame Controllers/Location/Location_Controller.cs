using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MaxSpawn_Phase
{
    [Range(0, 12)]
    public int timePoint;
    public int maxSpawnAmount;
}

public class Location_Controller : MonoBehaviour, ISaveLoadable
{
    [HideInInspector] public Main_Controller _mainController;

    [SerializeField] private SpriteRenderer _roamArea;
    public SpriteRenderer roamArea => _roamArea;

    [Header("Spawn Range Min to Max")]
    [SerializeField] private Vector2 _spawnXRange;
    [SerializeField] private Vector2 _spawnYRange;

    [Header("Spawn Time")]
    [SerializeField] private Vector2 _spawnIntervalTime;

    [SerializeField] private List<MaxSpawn_Phase> _maxSpawnPhase;
    private int _currentMaxSpawn;



    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentLocaiton(this);
    }

    private void Start()
    {
        // Toggle Off All Roam Area Colors On Game Start
        _roamArea.color = Color.clear;

        Update_Current_MaxSpawn();

        GlobalTime_Controller.TimeTik_Update += Update_Current_MaxSpawn;

        NPC_Spawn_Control();

        if (ES3.KeyExists("_currentMaxSpawn")) return;
        Spawn_Scraps(5);
    }



    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("_currentMaxSpawn", _currentMaxSpawn);
    }

    public void Load_Data()
    {
        _currentMaxSpawn = ES3.Load("_currentMaxSpawn", _currentMaxSpawn);
    }



    /// <returns>
    /// True if checkPosition is in restricted range, False if not
    /// </returns>
    public bool Restricted_Position(Vector2 checkPosition)
    {
        bool restricted = false;

        float xValue = checkPosition.x;
        if (xValue < _spawnXRange.x || xValue > _spawnXRange.y) restricted = true;

        float yValue = checkPosition.y;
        if (yValue < _spawnYRange.x || yValue > _spawnYRange.y) restricted = true;

        return restricted;
    }



    /// <summary>
    /// Also updates current npc population (global time tik update event)
    /// </summary>
    private void Update_Current_MaxSpawn()
    {
        for (int i = 0; i < _maxSpawnPhase.Count; i++)
        {
            if (_mainController.globalTime.currentTime != _maxSpawnPhase[i].timePoint) continue;

            _currentMaxSpawn = _maxSpawnPhase[i].maxSpawnAmount;
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
            if (!currentCharacters[i].TryGetComponent(out NPC_Controller npc)) continue;
            currentNPCs.Add(npc);
        }

        // if current npc amount is more than _currentMaxSpawn
        int npcOverFlowCount = currentCharacters.Count - _currentMaxSpawn;
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
        while (true)
        {
            while (_mainController.currentCharacters.Count >= _currentMaxSpawn) yield return null;

            float randIntervalTime = Random.Range(_spawnIntervalTime.x, _spawnIntervalTime.y);
            yield return new WaitForSeconds(randIntervalTime);

            _mainController.Spawn_Character(1, _mainController.OuterCamera_Position(Random.Range(0, 2)));
        }
    }



    // Scrap
    private void Spawn_Scraps(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randX = Random.Range((int)_spawnXRange.x, (int)_spawnXRange.y + 1);
            int randY = Random.Range((int)_spawnYRange.x, (int)_spawnYRange.y + 1);

            Vector2 spawnPos = new(randX, randY);

            if (_mainController.Position_Claimed(spawnPos))
            {
                i--;
                continue;
            }

            Station_Controller scrap = _mainController.Spawn_Station(6, spawnPos);
            scrap.movement.Load_Position();
        }
    }
}