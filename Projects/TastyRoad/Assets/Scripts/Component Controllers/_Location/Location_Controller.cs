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


    // UnityEngine
    private void Start()
    {
        // Toggle Off All Roam Area Colors On Game Start
        _data.roamArea.color = Color.clear;

        _maxPopulationData = _data.Max_PopulationData(GlobalTime_Controller.instance.currentTimePhase);

        Cycle_NPCSpawn();

        // subscriptions
        GlobalTime_Controller.instance.OnTimeTik += Update_MaxPopulation;
    }

    private void OnDestroy()
    {
        // subscriptions
        GlobalTime_Controller.instance.OnTimeTik -= Update_MaxPopulation;
    }


    // Gets
    /// <returns>
    /// True if checkPosition is in restricted range and position claimed, False if not
    /// </returns>
    public bool Restricted_Position(Vector2 checkPosition)
    {
        bool restricted = false;

        if (Main_Controller.instance.Position_Claimed(checkPosition)) restricted = true;

        float xValue = checkPosition.x;
        if (xValue < _data.spawnRangeX.x || xValue > _data.spawnRangeX.y) restricted = true;

        float yValue = checkPosition.y;
        if (yValue < _data.spawnRangeY.x || yValue > _data.spawnRangeY.y) restricted = true;

        return restricted;
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
        return Main_Controller.instance.SnapPosition(Redirected_Position(targetPosition));
    }


    public Vector2 Random_SnapPosition()
    {
        Main_Controller main = Main_Controller.instance;

        Vector2 randomPoint = main.Random_AreaPoint(_data.screenArea);
        Vector2 randSnapPos = main.SnapPosition(randomPoint);

        return randSnapPos;
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
        TimePhase currentTimePhase = _data.Max_PopulationData(GlobalTime_Controller.instance.currentTimePhase).timePhase;

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

            NPC_Movement move = npcs[i].movement;

            if (move.currentRoamArea != _data.roamArea) continue;
            if (npcOverFlowCount <= 0) break;

            npcOverFlowCount--;

            if (move.isLeaving)
            {
                npcOverFlowCount--;
                continue;
            }

            npcOverFlowCount--;
            move.Leave(0f);
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

            // set random theme skin for current location
            npcController.basicAnim.Set_OverrideController(_data.npcSkinOverrides[UnityEngine.Random.Range(0, _data.npcSkinOverrides.Length)]);

            // set npc free roam location
            npcController.movement.Free_Roam(_data.roamArea, 0f);
        }
    }
}