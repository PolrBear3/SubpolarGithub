using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location_Controller : MonoBehaviour
{
    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;

    [Header("")]
    [SerializeField] private LocationData _setData;
    public LocationData setData => _setData;

    private LocationData _currentData;
    public LocationData currentData => _currentData;

    [Header("")]
    [SerializeField] private Sprite _locationIcon;
    public Sprite locationIcon => _locationIcon;

    [Header("")]
    [SerializeField] private SpriteRenderer _environment;
    public SpriteRenderer environment => _environment;

    [SerializeField] private SpriteRenderer _roamArea;
    public SpriteRenderer roamArea => _roamArea;

    private MaxSpawn_TimePoint _currentTimePoint;

    [Header("")]
    [SerializeField] private List<GameObject> locationSetEvents = new();


    // UnityEngine
    private void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();

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

    private void OnDestroy()
    {
        GlobalTime_Controller.TimeTik_Update -= Update_Current_MaxSpawn;
    }


    // Current Data Control
    private void SetCurrent_LocationData()
    {
        _currentData = _setData;
    }


    // Gets
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
    /// Excludes claimed positions and restricted positions
    /// </summary>
    /// <returns>
    /// Random snap position of this location
    /// </returns>
    public Vector2 Location_Random_SnapPosition()
    {
        while (true)
        {
            Vector2 randSnapPos = Main_Controller.SnapPosition(Main_Controller.Random_AreaPoint(_environment));
            bool posAvailable = _mainController.Position_Claimed(randSnapPos) == false && Restricted_Position(randSnapPos) == false;

            if (posAvailable)
            {
                return randSnapPos;
            }

            continue;
        }
    }


    /// <summary>
    /// (-1, 0, 1) horizontal(left, right), vertical(top bottom), position x and y(local position)
    /// </summary>
    public Vector2 OuterLocation_Position(float horizontal, float vertical, float positionX, float positionY)
    {
        // Get the sprite's bounds
        Bounds bounds = _environment.bounds;

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

            _mainController.Spawn_Character(1, OuterLocation_Position(Random.Range(0, 2)));
        }
    }


    /// <summary>
    /// Activate all IInteractables in ingameEvents gameobject list
    /// </summary>
    public void Activate_LocationSet_Events()
    {
        for (int i = 0; i < locationSetEvents.Count; i++)
        {
            if (!locationSetEvents[i].TryGetComponent(out IInteractable ingameEvent)) continue;
            ingameEvent.Interact();
        }
    }
}