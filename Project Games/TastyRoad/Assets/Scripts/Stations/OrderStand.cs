using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class OrderStand : MonoBehaviour, IInteractable
{
    private SpriteRenderer _spriteRenderer;

    private Station_Controller _stationController;

    [SerializeField] private Action_Bubble _actionBubble;
    [SerializeField] private Clock_Timer _timer;

    [Header("Order Stand Sprites")]
    [SerializeField] private Sprite _openStand;
    [SerializeField] private Sprite _closedStand;

    [Header ("Action Bubble Sprites")]
    [SerializeField] private Sprite _lineOpenSprite;
    [SerializeField] private Sprite _lineClosedSprite;

    [Header("Order Control")]
    [SerializeField] private SpriteRenderer _orderingArea;

    [SerializeField] private int _maxWaitings;
    private List<NPC_Controller> _waitingNPCs = new();

    private Coroutine _attractCoroutine;
    [SerializeField] private float _attractIntervalTime;

    [Header ("Current Coin Display")]
    [SerializeField] private GameObject _coinDisplay;
    [SerializeField] private TextMeshPro _coinText;
    [SerializeField] private Sprite _coinSprite;
    [SerializeField] private Animator _coinAnimator;

    private Coroutine _coinCoroutine;
    [SerializeField] private float _displayTime;



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
        _coinDisplay.SetActive(false);
    }

    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _actionBubble.Toggle_Off();

        _stationController.Action1_Event -= Order_Toggle;
        _stationController.Action2_Event -= Show_CurrentCoin;
    }

    // IInteractable
    public void Interact()
    {
        _coinDisplay.SetActive(false);
        _timer.Toggle_Transparency(true);

        _actionBubble.Update_Bubble(ActionBubble_Sprite(), _coinSprite);

        _stationController.Action1_Event += Order_Toggle;
        _stationController.Action2_Event += Show_CurrentCoin;
    }

    public void UnInteract()
    {
        _actionBubble.Toggle_Off();
        _timer.Toggle_Transparency(false);

        _stationController.Action1_Event -= Order_Toggle;
        _stationController.Action2_Event -= Show_CurrentCoin;
    }



    // Get Line Toggle Sprite for Action Bubble
    private Sprite ActionBubble_Sprite()
    {
        // order closed
        if (Main_Controller.orderOpen == false)
        {
            return _lineOpenSprite;
        }

        // order open
        return _lineClosedSprite;
    }
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

    // Show Current Coin
    private IEnumerator Show_CurrentCoin_Coroutine()
    {
        _coinDisplay.SetActive(true);
        _coinText.text = Main_Controller.currentCoin.ToString();
        _coinAnimator.Play("Coin_collectStay");

        _timer.Toggle_Transparency(true);

        yield return new WaitForSeconds(_displayTime);

        _coinDisplay.SetActive(false);

        _timer.Toggle_Transparency(false);
    }
    private void Show_CurrentCoin()
    {
        _actionBubble.Toggle_Off();

        if (_coinCoroutine != null)
        {
            StopCoroutine(_coinCoroutine);
        }

        _coinCoroutine = StartCoroutine(Show_CurrentCoin_Coroutine());

        // reset action
        UnInteract();
    }



    private void Order_Toggle()
    {
        bool hasBookMark = _stationController.mainController.bookmarkedFoods.Count > 0;

        // order open
        if (Main_Controller.orderOpen == false && hasBookMark && _timer.timeRunning == false)
        {
            Main_Controller.orderOpen = true;

            Attract_allNPC();
        }

        // order closed
        else
        {
            Main_Controller.orderOpen = false;

            SpriteRenderer location = _stationController.mainController.currentLocation.roamArea;

            for (int i = 0; i < _waitingNPCs.Count; i++)
            {
                // return to current loaction free roam area
                NPC_Movement move = _waitingNPCs[i].movement;

                _waitingNPCs[i].timer.Toggle_Transparency(true);

                move.Stop_FreeRoam();
                move.Free_Roam(location, 4f);
            }

            // reset waiting npc list
            _waitingNPCs.Clear();

            if (hasBookMark && _timer.timeRunning == false)
            {
                // run cooltime
                _timer.Set_Time(Mathf.RoundToInt(_attractIntervalTime));
                _timer.Run_Time();
                _timer.Toggle_Transparency(false);
            }
        }

        // sprite update
        _spriteRenderer.sprite = Station_Sprite();

        // reset action
        UnInteract();
    }

    /// <summary>
    /// All current NPCc will decide every interval time wheather on they want to come and order food
    /// </summary>
    private void Attract_allNPC()
    {
        if (_attractCoroutine != null) StopCoroutine(_attractCoroutine);

        _attractCoroutine = StartCoroutine(Attract_allNPC_Coroutine());
    }
    private IEnumerator Attract_allNPC_Coroutine()
    {
        while(Main_Controller.orderOpen == true)
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
                if (!allCharacters[i].TryGetComponent(out NPC_Controller npc)) continue;

                NPC_Interaction interaction = npc.interaction;
                NPC_Movement move = npc.movement;

                // check if already ordered food
                if (npc.foodIcon.hasFood == true) continue;

                // check if food is already served and left ordering area
                if (interaction.foodOrderServed == true && move.currentRoamArea != _orderingArea)
                {
                    _waitingNPCs.Remove(npc);
                    continue;
                }

                // check if they want to order food
                if (interaction.Want_FoodOrder() == false) continue;

                // check max waiting npc amount
                if (_waitingNPCs.Count >= _maxWaitings) continue;

                // attract wake animtion
                interaction.Wake_Animation();

                // assign food order
                interaction.Assign_FoodOrder();

                // refresh
                interaction.UnInteract();

                // attract npc > ordering area
                move.Stop_FreeRoam();
                move.Free_Roam(_orderingArea, 0f);

                // keep track of currently waiting npc
                _waitingNPCs.Add(npc);
            }

            yield return new WaitForSeconds(_attractIntervalTime);
        }
    }
    //
    /// <summary>
    /// Removes items in list that are destroyed or missing
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