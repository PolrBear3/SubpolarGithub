using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationShopNPC : MonoBehaviour
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;

    [Header("Sub Location Properties")]
    [SerializeField] private SubLocation _currentSubLocation;

    [SerializeField] private Transform[] _boxStackPoints;
    [SerializeField] private StationStock[] _stationStocks;

    [Header("")]
    [SerializeField] private SpriteRenderer _carryBox;

    private Coroutine _restockCoroutine;


    // UnityEngine
    private void Start()
    {
        // untrack
        _npcController.mainController.UnTrack_CurrentCharacter(gameObject);

        // event subscriptions
        GlobalTime_Controller.DayTik_Update += Restock_StationStocks;
        _npcController.movement.TargetPosition_UpdateEvent += CarryBox_DirectionUpdate;

        // start free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        //
        CarryBox_SpriteToggle(false);
    }

    private void OnDestroy()
    {
        GlobalTime_Controller.DayTik_Update -= Restock_StationStocks;
        _npcController.movement.TargetPosition_UpdateEvent -= CarryBox_DirectionUpdate;
    }


    // Event Actions



    // Carry Box Sprite Control
    private void CarryBox_SpriteToggle(bool toggleOn)
    {
        if (toggleOn) _carryBox.color = Color.white;
        else _carryBox.color = Color.clear;
    }

    private void CarryBox_DirectionUpdate()
    {
        NPC_Movement move = _npcController.movement;

        // left
        if (move.Move_Direction() == -1) _carryBox.flipX = true;

        // right
        else _carryBox.flipX = false;
    }


    // Station Stock Control
    private bool StationStocks_Full()
    {
        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold) return false;
        }

        return true;
    }

    private void Restock_StationStocks()
    {
        if (StationStocks_Full()) return;

        if (_npcController.movement.roamActive == false) return;

        if (_restockCoroutine != null)
        {
            StopCoroutine(_restockCoroutine);
            _restockCoroutine = null;
        }

        _restockCoroutine = StartCoroutine(Restock_StationStocks_Coroutine());
    }
    private IEnumerator Restock_StationStocks_Coroutine()
    {
        NPC_Movement move = _npcController.movement;

        move.Stop_FreeRoam();

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold == false) continue;

            // Part 1 //

            // go to one of the box stacks
            Transform randBoxStack = _boxStackPoints[Random.Range(0, _boxStackPoints.Length)];
            move.Assign_TargetPosition(randBoxStack.position);

            // wait until destination reach
            while (move.At_TargetPosition() == false) yield return null;

            // carry box toggle on
            CarryBox_SpriteToggle(true);

            // Part 2 //

            // go to sold station stock
            move.Assign_TargetPosition(_stationStocks[i].transform.position);

            // wait until destination reach
            while (move.At_TargetPosition() == false) yield return null;

            // restock station stock
            _stationStocks[i].Restock();

            // carry box toggle off
            CarryBox_SpriteToggle(false);
        }

        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);
    }
}
