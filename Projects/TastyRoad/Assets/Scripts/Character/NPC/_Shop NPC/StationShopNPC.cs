using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationShopNPC : MonoBehaviour
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;
    [SerializeField] private ActionBubble_Interactable _interactable;

    [Header("")]
    [SerializeField] private SubLocation _currentSubLocation;

    [SerializeField] private Transform[] _boxStackPoints;
    [SerializeField] private StationStock[] _stationStocks;
    [SerializeField] private StationStock _mergeStationStock;

    [Header("")]
    [SerializeField] private SpriteRenderer _carryBox;

    private Coroutine _restockCoroutine;


    // UnityEngine
    private void Start()
    {
        // untrack
        _npcController.mainController.UnTrack_CurrentCharacter(gameObject);

        // event subscriptions
        GlobalTime_Controller.TimeTik_Update += Restock_StationStocks;
        _npcController.movement.TargetPosition_UpdateEvent += CarryBox_DirectionUpdate;

        _interactable.InteractEvent += Interact_FacePlayer;
        _interactable.Action1Event += Merge_BookMarkedStations;

        // start free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        //
        CarryBox_SpriteToggle(false);
    }

    private void OnDestroy()
    {
        // event subscriptions
        GlobalTime_Controller.TimeTik_Update -= Restock_StationStocks;
        _npcController.movement.TargetPosition_UpdateEvent -= CarryBox_DirectionUpdate;

        _interactable.InteractEvent -= Interact_FacePlayer;
        _interactable.Action1Event -= Merge_BookMarkedStations;
    }


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


    // Basic Actions
    private void Interact_FacePlayer()
    {
        // facing to player direction
        _npcController.basicAnim.Flip_Sprite(_npcController.interactable.detection.player.gameObject);

        NPC_Movement movement = _npcController.movement;

        movement.Stop_FreeRoam();
        movement.Free_Roam(_currentSubLocation.roamArea, Random.Range(movement.intervalTimeRange.x, movement.intervalTimeRange.y));
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

        _interactable.UnInteract();

        // interact lock
        _interactable.LockInteract(true);

        //
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

        // interact unlock
        _interactable.LockInteract(false);

        // return to free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);
    }


    // Merge Station Control
    private void Merge_BookMarkedStations()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // check if _mergeStationStock is empty
        if (_mergeStationStock.sold == false)
        {
            // dialog
            dialog.Update_Dialog(0);

            return;
        }

        //
        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        List<ItemSlot> bookMarkedSlots = menu.BookMarked_Slots();

        // check if there are more than 2 bookmarked stations
        if (bookMarkedSlots.Count < 2)
        {
            // dialog
            dialog.Update_Dialog(1);

            return;
        }

        // empty bookmarked stations
        for (int i = 0; i < bookMarkedSlots.Count; i++)
        {
            bookMarkedSlots[i].Empty_ItemBox();
        }

        //
        if (_restockCoroutine != null)
        {
            StopCoroutine(_restockCoroutine);
            _restockCoroutine = null;
        }

        _restockCoroutine = StartCoroutine(Merge_BookMarkedStations_Coroutine());
    }
    private IEnumerator Merge_BookMarkedStations_Coroutine()
    {
        NPC_Movement move = _npcController.movement;

        //
        CarryBox_SpriteToggle(true);

        // interact toggle off
        _interactable.LockInteract(true);

        // move to _mergeStationStock
        move.Assign_TargetPosition(_mergeStationStock.transform.position);

        // wait until arrival
        while (move.At_TargetPosition() == false) yield return null;

        // restock _mergeStationStock
        _mergeStationStock.Restock();

        // set _mergeStationStock price to 0
        _mergeStationStock.Update_Price(0);

        //
        CarryBox_SpriteToggle(false);

        // interact unlock
        _interactable.LockInteract(false);
    }
}
