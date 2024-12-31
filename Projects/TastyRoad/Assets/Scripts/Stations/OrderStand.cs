using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class OrderStand : MonoBehaviour, IInteractable
{
    private SpriteRenderer _spriteRenderer;

    private Station_Controller _stationController;

    [SerializeField] private Clock_Timer _timer;

    [Header("Order Stand Sprites")]
    [SerializeField] private Sprite _openStand;
    [SerializeField] private Sprite _closedStand;

    [Header("Action Bubble Sprites")]
    [SerializeField] private Sprite _lineOpenSprite;
    [SerializeField] private Sprite _lineClosedSprite;

    [Header("Order Control")]
    [SerializeField] private SpriteRenderer _orderingArea;
    public SpriteRenderer orderingArea => _orderingArea;

    [SerializeField] private int _maxWaitings;
    private List<NPC_Controller> _waitingNPCs = new();

    [SerializeField] private float _attractIntervalTime;
    private Coroutine _attractCoroutine;

    private bool _orderOpen;
    public bool orderOpen => _orderOpen;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _spriteRenderer = sr; }
        if (gameObject.TryGetComponent(out Station_Controller controller)) { _stationController = controller; }
    }

    private void Start()
    {
        _orderingArea.color = Color.clear;
        _timer.Toggle_Transparency(true);
    }


    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        UnInteract();
    }


    // IInteractable
    public void Interact()
    {
        if (_timer.timeRunning) return;

        OrderToggle();
    }

    public void Hold_Interact()
    {

    }

    public void UnInteract()
    {

    }


    // Gets
    private Sprite Station_Sprite()
    {
        // order closed
        if (Main_Controller.orderOpen == false)
        {
            return _closedStand;
        }

        // order open
        return _openStand;
    }


    // Functions
    private void OrderToggle()
    {
        bool hasBookMark = _stationController.mainController.bookmarkedFoods.Count > 0;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // if no bookmarked foods, return
        if (hasBookMark == false)
        {
            dialog.Update_Dialog(2);
            return;
        }

        // if other order stand currenlty toggled on, return
        if (Main_Controller.orderOpen == true && _orderOpen == false)
        {
            dialog.Update_Dialog(3);
            return;
        }

        // order open
        if (Main_Controller.orderOpen == false)
        {
            Main_Controller.OrderOpen_Toggle(true);
            _orderOpen = true;

            _stationController.RoamArea_Toggle(true);

            GlobalTime_Controller.TimeTik_Update += TimeTik_Attract;
            dialog.Update_Dialog(0);
        }

        // order closed
        else
        {
            // order stand toggle cooltime
            _timer.Set_Time((int)_attractIntervalTime);
            _timer.Run_Time();
            _timer.Toggle_Transparency(false);

            // order close
            Main_Controller.OrderOpen_Toggle(false);
            _orderOpen = false;

            _stationController.RoamArea_Toggle(false);

            // subscription update
            GlobalTime_Controller.TimeTik_Update -= TimeTik_Attract;
            dialog.Update_Dialog(1);

            //
            SpriteRenderer location = _stationController.mainController.currentLocation.data.roamArea;

            // waiting npc update
            for (int i = 0; i < _waitingNPCs.Count; i++)
            {
                _waitingNPCs[i].timer.Toggle_Transparency(true);

                // set time limit current time to 0 to activate TimeLimit_Over() from npc interaction
                _waitingNPCs[i].timer.Update_CurrentTime(-(int)_waitingNPCs[i].timer.currentTime);

                // reset all npc roam area (just in case for if not updating on order close)
                _waitingNPCs[i].movement.Free_Roam(location);
            }

            //
            _waitingNPCs.Clear();
        }

        // sprite update
        _spriteRenderer.sprite = Station_Sprite();

        //
        UnInteract();
    }


    private void TimeTik_Attract()
    {
        // check if order is open
        if (Main_Controller.orderOpen == false) return;

        // check if there is at least 1 food bookmarked
        if (_stationController.mainController.bookmarkedFoods.Count <= 0) return;

        // all current npc
        List<GameObject> allCharacters = _stationController.mainController.currentCharacters;

        Refresh_WaitingNPCs();

        for (int i = 0; i < allCharacters.Count; i++)
        {
            // check max waiting npc amount
            if (_waitingNPCs.Count >= _maxWaitings) continue;

            if (!allCharacters[i].TryGetComponent(out NPC_Controller npc)) continue;

            NPC_Interaction interaction = npc.interaction;
            if (interaction == null) continue;

            NPC_Movement move = npc.movement;

            // check if already ordered food
            if (npc.foodIcon.hasFood) continue;

            // check if food is already served and left ordering area
            if (interaction.servedFoodData != null && move.currentRoamArea != _orderingArea)
            {
                _waitingNPCs.Remove(npc);
                continue;
            }

            // check if npc want to order food
            if (interaction.Want_FoodOrder() == false) continue;

            // keep track of currently waiting npc
            _waitingNPCs.Add(npc);

            // assign food
            interaction.Assign_FoodOrder();

            // attract wake animtion
            interaction.Wake_Animation();

            // refresh
            npc.interactable.UnInteract();

            // attract npc > ordering area
            move.Stop_FreeRoam();
            move.Free_Roam(_orderingArea, 0f);

            // start waiting time limit
            interaction.Start_TimeLimit();
            npc.timer.Toggle_Transparency(false, interaction.animTransitionTime);
        }
    }

    /// <summary>
    /// All current NPCc will decide every interval time wheather on they want to come and order food
    /// </summary>
    private void Attract()
    {
        if (_attractCoroutine != null) StopCoroutine(_attractCoroutine);

        _attractCoroutine = StartCoroutine(Attract_Coroutine());
    }
    private IEnumerator Attract_Coroutine()
    {
        while (Main_Controller.orderOpen == true)
        {
            while (_stationController.mainController.bookmarkedFoods.Count <= 0)
            {
                yield return null;
            }

            // all current npc
            List<GameObject> allCharacters = _stationController.mainController.currentCharacters;

            Refresh_WaitingNPCs();

            for (int i = 0; i < allCharacters.Count; i++)
            {
                // check max waiting npc amount
                if (_waitingNPCs.Count >= _maxWaitings) continue;

                if (!allCharacters[i].TryGetComponent(out NPC_Controller npc)) continue;

                NPC_Interaction interaction = npc.interaction;
                NPC_Movement move = npc.movement;

                // check if already ordered food
                if (npc.foodIcon.hasFood) continue;

                // check if food is already served and left ordering area
                if (interaction.servedFoodData != null && move.currentRoamArea != _orderingArea)
                {
                    _waitingNPCs.Remove(npc);
                    continue;
                }

                // check if npc want to order food
                if (interaction.Want_FoodOrder() == false) continue;

                // keep track of currently waiting npc
                _waitingNPCs.Add(npc);

                // assign food
                interaction.Assign_FoodOrder();

                // attract wake animtion
                interaction.Wake_Animation();

                // refresh
                npc.interactable.UnInteract();

                // attract npc > ordering area
                move.Stop_FreeRoam();
                move.Free_Roam(_orderingArea, 0f);

                // start waiting time limit
                interaction.Start_TimeLimit();
                npc.timer.Toggle_Transparency(false, interaction.animTransitionTime);
            }

            yield return new WaitForSeconds(_attractIntervalTime);
        }
    }


    /// <summary>
    /// Removes npcs in list that are destroyed or missing
    /// </summary>
    private void Refresh_WaitingNPCs()
    {
        List<NPC_Controller> refreshList = new();

        for (int i = 0; i < _waitingNPCs.Count; i++)
        {
            NPC_Controller target = _waitingNPCs[i];

            // npc left current location
            if (target == null) continue;

            // waiting npc time expired
            if (target.foodIcon.hasFood == false) continue;

            refreshList.Add(_waitingNPCs[i]);
        }

        _waitingNPCs = refreshList;
        // Debug.Log("waiting list refresh complete");
    }
}