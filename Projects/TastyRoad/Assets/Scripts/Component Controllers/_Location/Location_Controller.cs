using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Location_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private LocationData _data;
    public LocationData data => _data;

    [Header("Subscribe on Inspector")]
    [SerializeField] private UnityEvent _OnFirstVisit;
    public UnityEvent OnFirstVisit => _OnFirstVisit;

    [SerializeField] private UnityEvent _OnNewLocationSet;
    public UnityEvent OnNewLocationSet => _OnNewLocationSet;


    private TimePhase_Population _maxPopulationData;

    private List<NPC_Controller> _foodOrderNPCs = new();
    public List<NPC_Controller> foodOrderNPCs => _foodOrderNPCs;


    // UnityEngine
    private void Start()
    {
        // Toggle Off All Roam Area Colors On Game Start
        _data.roamArea.color = Color.clear;

        _maxPopulationData = _data.Max_PopulationData(globaltime.instance.currentTimePhase);

        _data.UpdateCurrent_FoodOrderCount(_data.maxFoodOrderCount);
        Cycle_NPCSpawn();

        // subscriptions
        globaltime.instance.OnTimeTik += Update_MaxPopulation;
    }

    private void OnDestroy()
    {
        // subscriptions
        globaltime.instance.OnTimeTik -= Update_MaxPopulation;
    }


    // Position Checks
    /// <returns>
    /// True if checkPosition is in restricted range and position claimed, False if not
    /// </returns>
    public bool Restricted_Position(Vector2 checkPosition)
    {
        bool restricted = false;

        float xValue = checkPosition.x;
        if (xValue < _data.spawnRangeX.x || xValue > _data.spawnRangeX.y) restricted = true;

        float yValue = checkPosition.y;
        if (yValue < _data.spawnRangeY.x || yValue > _data.spawnRangeY.y) restricted = true;

        return restricted;
    }


    // Position Utilities
    public List<Vector2> All_SpawnPositions()
    {
        Main_Controller main = Main_Controller.instance;

        List<Vector2> spawnPositions = new();

        Vector2 rangeX = _data.spawnRangeX;
        Vector2 rangeY = _data.spawnRangeY;

        for (int x = (int)rangeX.x; x <= (int)rangeX.y; x++)
        {
            for (int y = (int)rangeY.x; y <= (int)rangeY.y; y++)
            {
                Vector2 spawnPos = Utility.SnapPosition(new Vector2(x, y));
                spawnPositions.Add(spawnPos);
            }
        }

        return spawnPositions;
    }

    /// <summary>
    /// Returns a random, non claimed, spawn position
    /// </summary>
    public Vector2 Random_SpawnPosition()
    {
        Main_Controller main = Main_Controller.instance;

        List<Vector2> spawnPositions = All_SpawnPositions();

        while (spawnPositions.Count > 0)
        {
            int randIndex = UnityEngine.Random.Range(0, spawnPositions.Count);
            Vector2 randPos = spawnPositions[randIndex];

            if (main.Position_Claimed(randPos))
            {
                spawnPositions.RemoveAt(randIndex);
                continue;
            }

            return randPos;
        }

        return Vector2.zero;
    }


    /// <returns>
    /// closest available location from restrictedPosition
    /// </returns>
    public Vector2 Redirected_Position(Vector2 restrictedPosition)
    {
        if (Restricted_Position(restrictedPosition) == false) return restrictedPosition;

        float closestXPos = Mathf.Clamp(restrictedPosition.x, _data.spawnRangeX.x, _data.spawnRangeX.y);
        float closestYPos = Mathf.Clamp(restrictedPosition.y, _data.spawnRangeY.x, _data.spawnRangeY.y);

        return new Vector2(closestXPos, closestYPos);
    }

    public Vector2 Redirected_SnapPosition(Vector2 targetPosition)
    {
        return Utility.SnapPosition(Redirected_Position(targetPosition));
    }


    /// <summary>
    /// (-1, 0, 1) horizontal(left, right), vertical(top bottom), position x and y(local position)
    /// </summary>
    public Vector2 OuterLocation_Position(float horizontal, float vertical, float positionX, float positionY)
    {
        // Get the sprite's bounds
        Bounds bounds = _data.screenArea.bounds;

        // Calculate the horizontal position
        float horizontalPos = horizontal;
        if (horizontalPos == 0) horizontalPos = 0.5f;
        else if (horizontalPos == -1f) horizontalPos = 0f;
        else if (horizontalPos == 1f) horizontalPos = 1f;

        // Calculate the vertical position
        float verticalPos = vertical;
        if (verticalPos == 0) verticalPos = 0.5f;
        else if (verticalPos == -1f) verticalPos = 0f;
        else if (verticalPos == 1f) verticalPos = 1f;

        // Calculate the world position based on the bounds and the local position
        Vector2 spritePosition = new Vector2(
            Mathf.Lerp(bounds.min.x, bounds.max.x, horizontalPos),
            Mathf.Lerp(bounds.min.y, bounds.max.y, verticalPos)
        );

        // Apply the additional local position offsets
        return new Vector2(spritePosition.x + positionX, spritePosition.y + positionY);
    }

    /// <summary>
    /// left is -1, right is 1
    /// </summary>
    public Vector2 OuterLocation_Position(int leftRight)
    {
        // left
        if (leftRight <= 0) return OuterLocation_Position(-1, 0, -2, -3);

        // right
        else return OuterLocation_Position(1, 0, 2, -3);
    }


    // NPC Control
    private List<NPC_Controller> Current_npcControllers()
    {
        List<GameObject> currentCharacters = Main_Controller.instance.currentCharacters;
        List<NPC_Controller> currentNPCs = new();

        for (int i = 0; i < currentCharacters.Count; i++)
        {
            if (!currentCharacters[i].TryGetComponent(out NPC_Controller npc)) continue;
            if (npc.foodInteraction == null) continue;

            currentNPCs.Add(npc);
        }

        return currentNPCs;
    }


    /// <summary>
    /// for _maxPopulation data update
    /// </summary>
    private void Update_MaxPopulation()
    {
        TimePhase currentTimePhase = _data.Max_PopulationData(globaltime.instance.currentTimePhase).timePhase;

        if (_maxPopulationData.timePhase == currentTimePhase) return;

        _maxPopulationData = _data.Max_PopulationData(currentTimePhase);
    }

    /// <summary>
    /// for leave (decrease)
    /// </summary>
    private void Decrease_npcPopulation()
    {
        List<NPC_Controller> npcs = Current_npcControllers();

        // if current npc amount is more than _currentMaxSpawn
        int npcOverFlowCount = npcs.Count - _maxPopulationData.maxPopulation;

        if (npcOverFlowCount <= 0) return;

        // current location roaming npc only leave
        for (int i = 0; i < npcs.Count; i++)
        {
            if (npcs[i].foodInteraction == null) continue;

            NPC_Movement movement = npcs[i].movement;

            if (movement.currentRoamArea != _data.roamArea) continue;
            if (npcOverFlowCount <= 0) break;

            npcOverFlowCount--;

            if (movement.isLeaving)
            {
                npcOverFlowCount--;
                continue;
            }

            npcOverFlowCount--;
            movement.Leave(0f);
        }
    }


    /// <summary>
    /// for spawn
    /// </summary>
    public void Cycle_NPCSpawn()
    {
        StartCoroutine(Cycle_NPCSpawn_Coroutine());
    }
    private IEnumerator Cycle_NPCSpawn_Coroutine()
    {
        while (true)
        {
            while (Current_npcControllers().Count >= _maxPopulationData.maxPopulation)
            {
                Decrease_npcPopulation();

                yield return new WaitForSeconds(1);
                yield return null;
            }

            yield return new WaitForSeconds(_data.spawnIntervalTime);

            // spawn
            GameObject spawnNPC = Main_Controller.instance.Spawn_Character(1, OuterLocation_Position(UnityEngine.Random.Range(0, 2)));

            // get npc controller
            NPC_Controller npcController = spawnNPC.GetComponent<NPC_Controller>();
            NPC_Movement movement = npcController.movement;

            // set random theme skin for current location
            npcController.basicAnim.Set_OverrideController(_data.Random_NPCSkinOverride());

            // set npc free roam location
            movement.Free_Roam(_data.roamArea, movement.Random_IntervalTime());
        }
    }


    public void Track_FoodOrderNPC(NPC_Controller trackNPC, bool trackToggle)
    {
        if (trackNPC == null)
        {
            Debug.Log("trackNPC is null");
            return;
        }
        
        if (trackToggle && _foodOrderNPCs.Contains(trackNPC) == false)
        {
            _foodOrderNPCs.Add(trackNPC);
            return;
        }

        if (trackToggle || _foodOrderNPCs.Contains(trackNPC) == false) return;
        
        _foodOrderNPCs.Remove(trackNPC);
    }
    
    public bool FoodOrderNPC_Maxed()
    {
        if (_foodOrderNPCs.Count >= _maxPopulationData.maxPopulation) return false;
        
        return _foodOrderNPCs.Count >= _data.currentFoodOrderCount;
    }
}