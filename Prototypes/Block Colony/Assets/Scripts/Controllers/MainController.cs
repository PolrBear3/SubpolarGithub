using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public interface IInteractCheck
{
    bool InteractAvailable();
}

public class MainController : MonoBehaviour
{
    private CurrentGameData _gameData = new();
    public CurrentGameData gameData => _gameData;

    public static bool actionAvailable = true;


    [Header("")]
    [SerializeField] private GlobalVolume_Controller _postProcessing;
    public GlobalVolume_Controller postProcessing => _postProcessing;

    [SerializeField] private MenuController _menu;
    public MenuController menu => _menu;

    [SerializeField] private DataController _data;
    public DataController data => _data;

    [SerializeField] private CardsController _cards;
    public CardsController cards => _cards;

    [SerializeField] private Cursor _cursor;
    public Cursor cursor => _cursor;

    [SerializeField] private ToolTip _toolTip;
    public ToolTip toolTip => _toolTip;


    [Header("")]
    [SerializeField] private Land_SnapPoint[] _snapPoints;
    public Land_SnapPoint[] snapPoints => _snapPoints;


    [Header("")]
    [SerializeField] private TextMeshProUGUI _overallPopulationText;
    [SerializeField] private TextMeshProUGUI _updatePopulationText;
    [SerializeField] private TextMeshProUGUI _turnsText;


    [Header("New Game Data")]
    [SerializeField] [Range(0, 6)] private int _startingCardAmount;

    [SerializeField] private int _overallPopulationGoal;
    public int overallPopulationGoal => _overallPopulationGoal;


    // UnityEngine
    private void Start()
    {
        Set_SnapPoint_Datas();

        // new game

        // set land on random point
        Set_StartingLand();

        //
        _turnsText.text = _gameData.turnCount.ToString();

        // start with _startingCardAmount of land cards in deck
        _cards.AddCards_toDeck(_data.AllLandCard_ScrObjs(_startingCardAmount));
    }


    // Get Current Lands
    public Land Get_Land(Vector2 gridNum)
    {
        for (int i = 0; i < _snapPoints.Length; i++)
        {
            if (_snapPoints[i].currentData.hasLand == false) continue;
            if (gridNum != _snapPoints[i].currentData.gridNum) continue;

            return _snapPoints[i].currentData.currentLand;
        }
        return null;
    }

    public List<Land> Lands()
    {
        List<Land> lands = new();

        for (int i = 0; i < _snapPoints.Length; i++)
        {
            if (_snapPoints[i].currentData.hasLand == false) continue;
            lands.Add(_snapPoints[i].currentData.currentLand);
        }

        return lands;
    }

    public List<Land> OffSet_Lands(Land land, List<Vector2> positions)
    {
        Vector2 gridSize = _snapPoints[_snapPoints.Length - 1].currentData.gridNum;
        List<Land> surroundingLands = new();

        foreach (Vector2 position in positions)
        {
            float xNum = land.currentData.snapPoint.currentData.gridNum.x + position.x;
            float yNum = land.currentData.snapPoint.currentData.gridNum.y + position.y;

            // check if grid is inside grid
            if (xNum < 0 && xNum > gridSize.x) continue;
            if (yNum < 0 && yNum > gridSize.y) continue;

            Land searchLand = Get_Land(new Vector2(xNum, yNum));

            // Check if the land exists
            if (searchLand == null) continue;

            // condition success
            surroundingLands.Add(searchLand);
        }

        return surroundingLands;
    }

    public List<Land> CrossSurrounding_Lands(Land_SnapPoint snapPoint)
    {
        Vector2 gridSize = _snapPoints[_snapPoints.Length - 1].currentData.gridNum;
        List<Land> surroundingLands = new();

        float[] xOffset = { -1, 0, 1, 0 };
        float[] yOffset = { 0, 1, 0, -1 };

        for (int i = 0; i < xOffset.Length; i++)
        {
            float xNum = snapPoint.currentData.gridNum.x + xOffset[i];
            float yNum = snapPoint.currentData.gridNum.y + yOffset[i];

            // check if grid is inside grid
            if (xNum < 0 && xNum > gridSize.x) continue;
            if (yNum < 0 && yNum > gridSize.y) continue;

            Land searchLand = Get_Land(new Vector2(xNum, yNum));

            // check if land exists
            if (searchLand == null) continue;

            // condition success
            surroundingLands.Add(searchLand);
        }

        return surroundingLands;
    }
    public List<Land> CrossSurrounding_Lands(Land land)
    {
        Vector2 gridSize = _snapPoints[_snapPoints.Length - 1].currentData.gridNum;
        List<Land> surroundingLands = new();

        float[] xOffset = { -1, 0, 1, 0 };
        float[] yOffset = { 0, 1, 0, -1 };

        for (int i = 0; i < xOffset.Length; i++)
        {
            float xNum = land.currentData.snapPoint.currentData.gridNum.x + xOffset[i];
            float yNum = land.currentData.snapPoint.currentData.gridNum.y + yOffset[i];

            // check if grid is inside grid
            if (xNum < 0 && xNum > gridSize.x) continue;
            if (yNum < 0 && yNum > gridSize.y) continue;

            Land searchLand = Get_Land(new Vector2(xNum, yNum));

            // check if land exists
            if (searchLand == null) continue;

            // condition success
            surroundingLands.Add(searchLand);
        }

        return surroundingLands;
    }


    // Set Functions
    private void Set_SnapPoint_Datas()
    {
        int xCount = 0;
        int yCount = 0;

        for (int i = 0; i < _snapPoints.Length; i++)
        {
            SnapPointData setData = new(new Vector2(xCount, yCount));
            _snapPoints[i].Set_CurrentData(setData);

            xCount++;

            if (i == 0) continue;
            if (i == _snapPoints.Length - 1) continue;
            if (_snapPoints[i].transform.position.y == _snapPoints[i + 1].transform.position.y) continue;

            xCount = 0;
            yCount++;
        }
    }

    private void Set_StartingLand()
    {
        Land_SnapPoint randSnapPoint = _snapPoints[Random.Range(0, _snapPoints.Length)];

        GameObject spawn = Instantiate(_data.landPrefab, randSnapPoint.transform.position, Quaternion.identity);
        Land spawnLand = spawn.GetComponent<Land>();

        spawnLand.Set_CurrentData(new LandData(randSnapPoint, LandType.plain));
        randSnapPoint.currentData.Update_CurrentLand(spawnLand);

        spawn.transform.SetParent(randSnapPoint.transform);

        // current population update
        Update_UpdatePopulation();
    }


    // Current Game Data Population control
    private void Update_BonusPopulation(bool isReset)
    {
        for (int i = 0; i < Lands().Count; i++)
        {
            LandData data = Lands()[i].currentData;

            if (isReset)
            {
                data.Update_Population(-data.bonusPopulation);
                data.Update_BonusPopulation(-data.bonusPopulation);
                continue;
            }

            data.Update_Population(data.bonusPopulation);
        }
    }

    /// <summary>
    /// Calculates all current land populations
    /// </summary>
    public void Update_UpdatePopulation()
    {
        int calculateCount = 0;

        for (int i = 0; i < Lands().Count; i++)
        {
            calculateCount += Lands()[i].currentData.population;

            _gameData.updatePopulation = calculateCount;
            CurrentPopulation_TextUpdate();
        }
    }


    /// <summary>
    /// overall population += update population
    /// </summary>
    private void Update_OverallPopulation()
    {
        _gameData.overallPopulation += _gameData.updatePopulation;

        CurrentPopulation_TextUpdate();
    }
    public void Update_OverallPopulation(int updateAmount)
    {
        _gameData.overallPopulation += updateAmount;
        CurrentPopulation_TextUpdate();
    }

    public void CurrentPopulation_TextUpdate()
    {
        _overallPopulationText.text = _gameData.overallPopulation.ToString();
        _updatePopulationText.text = "+" + _gameData.updatePopulation.ToString();
    }


    // Land Control
    private void Activate_LandEvents()
    {
        foreach (var land in Lands())
        {
            land.events.Activate_AllEvents();
        }
    }

    public void Play_LandEvents_Animation()
    {
        foreach (var land in Lands())
        {
            land.events.Play_CurrentEvents_Animation();
        }
    }


    // Turn Control
    private void Check_GameOver()
    {
        if (_menu.isContinuePlay) return;
        if (_gameData.overallPopulation < _overallPopulationGoal) return;

        // end game menu
        _menu.EndGame();
    }

    public void NextTurn_Invoke()
    {
        if (actionAvailable == false) return;

        StartCoroutine(NextTurn_Invoke_Coroutine());
    }
    private IEnumerator NextTurn_Invoke_Coroutine()
    {
        actionAvailable = false;

        _cursor.Clear_Card();
        _cards.ReturnDrawnCards_toDeck();

        // wait until drawn cards are empty for animation (check if first drawn card is empty)
        while (_cards.snapPoints[0].hasCard != false) yield return null;

        _cards.DrawCard_fromDeck(_cards.maxDrawCardAmount);

        // bonud population reset
        Update_BonusPopulation(true);

        // land events
        Activate_LandEvents();
        Play_LandEvents_Animation();

        // bonud population update
        Update_BonusPopulation(false);

        // population
        Update_UpdatePopulation();
        Update_OverallPopulation();

        //
        _gameData.turnCount++;
        _turnsText.text = _gameData.turnCount.ToString();

        Check_GameOver();

        actionAvailable = true;
    }
}
