using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
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


    public delegate void Event();
    public event Event NextTurn;


    // UnityEngine
    private void Start()
    {
        Set_SnapPoint_Datas();

        for (int i = 0; i < 6; i++)
        {
            cards.AddCard_toDeck();

            _cards.DrawCard_fromDeck();
        }
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


    // Turn Control
    public void NextTurn_Invoke()
    {
        NextTurn?.Invoke();

        _cards.DrawCard_fromDeck();
    }
}
