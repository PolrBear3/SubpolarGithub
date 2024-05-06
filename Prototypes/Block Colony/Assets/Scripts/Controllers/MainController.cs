using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainController : MonoBehaviour
{
    private CurrentGameData _gameData = new();
    public CurrentGameData gameData => _gameData;


    [Header("")]
    [SerializeField] private DataController _data;
    public DataController data => _data;

    [SerializeField] private CardsController _cards;
    public CardsController cards => _cards;

    [SerializeField] private Cursor _cursor;
    public Cursor cursor => _cursor;


    [Header("")]
    [SerializeField] private Land_SnapPoint[] _snapPoints;
    public Land_SnapPoint[] snapPoints => _snapPoints;


    [Header("")]
    [SerializeField] private TextMeshProUGUI _overallPopulationText;
    [SerializeField] private TextMeshProUGUI _updatePopulationText;


    // UnityEngine
    private void Start()
    {
        Set_SnapPoint_Datas();

        CurrentPopulation_TextUpdate();
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


    /// <summary>
    /// Calculates all current land populations
    /// </summary>
    public void Update_UpdatePopulation()
    {
        int calculateCount = 0;

        for (int i = 0; i < _snapPoints.Length; i++)
        {
            if (_snapPoints[i].currentData.hasLand == false) continue;
            calculateCount += _snapPoints[i].currentData.currentLand.currentData.population;
        }
        _gameData.updatePopulation = calculateCount;

        CurrentPopulation_TextUpdate();
    }

    /// <summary>
    /// overall population += update population
    /// </summary>
    private void Update_OverallPopulation()
    {
        _gameData.overallPopulation += _gameData.updatePopulation;

        CurrentPopulation_TextUpdate();
    }


    public void CurrentPopulation_TextUpdate()
    {
        _overallPopulationText.text = _gameData.overallPopulation.ToString();
        _updatePopulationText.text = "+" + _gameData.updatePopulation.ToString();
    }


    // Current Placed Land Control
    private void Activate_LandEvents()
    {
        for (int i = 0; i < _snapPoints.Length; i++)
        {
            if (snapPoints[i].currentData.hasLand == false) continue;
            Land currentLand = snapPoints[i].currentData.currentLand;

            currentLand.events.Activate_AllEvents();
        }
    }

    private void NegativePopulation_LandEvents()
    {
        for (int i = 0; i < _snapPoints.Length; i++)
        {
            if (snapPoints[i].currentData.hasLand == false) continue;
            Land currentLand = snapPoints[i].currentData.currentLand;

            // check if population is bellow 0
            if (currentLand.currentData.population >= 0) continue;

            // destroy land
            snapPoints[i].currentData.Update_CurrentLand(null);
            Destroy(currentLand.gameObject);
        }
    }


    // Turn Control
    public void NextTurn_Invoke()
    {
        // land events
        Activate_LandEvents();
        NegativePopulation_LandEvents();

        // population calculation update
        Update_UpdatePopulation();
        Update_OverallPopulation();

        // draw card
        _cards.DrawCard_fromDeck(6);

        //
        _gameData.turnCount++;
    }
}
