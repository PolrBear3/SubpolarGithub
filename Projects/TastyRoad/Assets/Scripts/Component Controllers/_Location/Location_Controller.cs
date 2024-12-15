using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location_Controller : MonoBehaviour
{
    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;


    [Header("")]
    [SerializeField] private LocationData _data;
    public LocationData data => _data;

    [Header("")]
    [SerializeField] private GameObject[] _locationSetEvents;


    private MaxSpawn_TimePoint _currentTimePoint;


    // UnityEngine
    private void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }

    private void Start()
    {
        // Toggle Off All Roam Area Colors On Game Start
        _data.roamArea.color = Color.clear;

        Update_Current_MaxSpawn();
        GlobalTime_Controller.TimeTik_Update += Update_Current_MaxSpawn;

        NPC_Spawn_Control();
    }

    private void OnDestroy()
    {
        GlobalTime_Controller.TimeTik_Update -= Update_Current_MaxSpawn;
    }


    // Gets
    /// <returns>
    /// True if checkPosition is in restricted range and position claimed, False if not
    /// </returns>
    public bool Restricted_Position(Vector2 checkPosition)
    {
        bool restricted = false;

        if (_mainController.Position_Claimed(checkPosition)) restricted = true;

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
        return Main_Controller.SnapPosition(Redirected_Position(targetPosition));
    }


    public Vector2 Random_SnapPosition()
    {
        Vector2 randSnapPos = Main_Controller.SnapPosition(Main_Controller.Random_AreaPoint(_data.screenArea));
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
    /// <summary>
    /// Also updates current npc population (global time tik update event)
    /// </summary>
    private void Update_Current_MaxSpawn()
    {
        for (int i = 0; i < _data.maxSpawnTimePoints.Count; i++)
        {
            if (_mainController.globalTime.currentTime != _data.maxSpawnTimePoints[i].timePoint) continue;

            _currentTimePoint = _data.maxSpawnTimePoints[i];
            Update_NPC_Population();

            return;
        }
    }

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
        int npcOverFlowCount = currentCharacters.Count - _currentTimePoint.maxSpawnAmount;
        if (npcOverFlowCount <= 0) return;

        // current location roaming npc only leave
        for (int i = 0; i < currentNPCs.Count; i++)
        {
            NPC_Movement move = currentNPCs[i].movement;

            if (move.currentRoamArea != _data.roamArea) continue;
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
            while (_mainController.currentCharacters.Count >= _currentTimePoint.maxSpawnAmount) yield return null;

            float randIntervalTime = Random.Range(_data.spawnIntervalTimeRange.x, _data.spawnIntervalTimeRange.y);
            yield return new WaitForSeconds(randIntervalTime);

            // spawn
            GameObject spawnNPC = _mainController.Spawn_Character(1, OuterLocation_Position(Random.Range(0, 2)));

            // get npc controller
            NPC_Controller npcController = spawnNPC.GetComponent<NPC_Controller>();

            // set random theme skin for current location
            npcController.basicAnim.Set_OverrideController(_data.npcSkinOverrides[Random.Range(0, _data.npcSkinOverrides.Length)]);

            // set npc free roam location
            npcController.movement.Free_Roam(_data.roamArea, 0f);
        }
    }


    /// <summary>
    /// New Location Set Starting Events Control
    /// </summary>
    public void Activate_NewSetEvents()
    {
        for (int i = 0; i < _locationSetEvents.Length; i++)
        {
            if (_locationSetEvents[i].TryGetComponent(out IInteractable interactable) == false) continue;
            interactable.Interact();
        }
    }
}