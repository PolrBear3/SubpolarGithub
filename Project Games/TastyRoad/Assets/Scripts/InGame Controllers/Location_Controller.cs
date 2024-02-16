using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MaxSpawn_Phase
{
    public int timePoint;
    public int maxSpawnAmount;
}

public class Location_Controller : MonoBehaviour
{
    [HideInInspector] public Main_Controller _mainController;

    [SerializeField] private SpriteRenderer _roamArea;
    public SpriteRenderer roamArea => _roamArea;

    [Header("")]
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

        _mainController.globalTime.TimeTik_Update += Update_Current_MaxSpawn;
        NPC_Spawn_Control();
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
        int npcOverFlowCount = currentNPCs.Count - _currentMaxSpawn;
        if (npcOverFlowCount <= 0) return;

        // current location roaming npc only leave
        for (int i = 0; i < currentNPCs.Count; i++)
        {
            NPC_Movement move = currentNPCs[i].movement;

            if (move.currentRoamArea != _roamArea) continue;
            if (move.isLeaving) continue;

            move.Leave(0f);
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
}