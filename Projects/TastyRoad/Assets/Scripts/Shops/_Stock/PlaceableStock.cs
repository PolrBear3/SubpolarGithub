using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private ActionBubble_Interactable _interactable;
    public ActionBubble_Interactable interactable => _interactable;

    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [Header("")]
    [SerializeField] private Sprite[] _sprites;

    [Header("")]
    [SerializeField][Range(1, 98)] private int _maxAmount;


    private bool _isComplete;
    public bool isComplete => _isComplete;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Update_Bubble();
        Update_forComplete();

        // subscriptions
        _interactable.detection.EnterEvent += Toggle_AmountBar;
        _interactable.detection.ExitEvent += Toggle_AmountBar;

        _interactable.OnIInteract += Toggle_AmountBar;
        _interactable.OnUnIInteract += Toggle_AmountBar;

        _interactable.OnHoldIInteract += Transfer_CurrentFood;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.detection.EnterEvent -= Toggle_AmountBar;
        _interactable.detection.ExitEvent -= Toggle_AmountBar;

        _interactable.OnIInteract -= Toggle_AmountBar;
        _interactable.OnUnIInteract -= Toggle_AmountBar;

        _interactable.OnHoldIInteract += Transfer_CurrentFood;

        _interactable.OnAction1Input -= Complete;
        _interactable.OnAction1Input -= Place;
    }


    // Data Control
    public void Load_Data(List<FoodData> placedData, bool completeData)
    {
        _foodIcon.Update_AllDatas(placedData);
        _isComplete = completeData;
    }

    public void Reset_Data()
    {
        _foodIcon.Update_AllDatas(null);
        _foodIcon.Show_Icon();

        _isComplete = false;
        Update_forComplete();
    }


    // Updates and Toggles
    public void Update_forComplete() // currently used in test editor
    {
        _foodIcon.Toggle_Height(_isComplete);
        _interactable.bubble.Toggle_Height(_isComplete);

        if (_isComplete == false)
        {
            _sr.sprite = _sprites[0];
            _foodIcon.ShowIcon_LockToggle(false);

            _foodIcon.Toggle_Height(false);

            return;
        }

        _sr.sprite = _sprites[1];
        _foodIcon.Hide_Icon();
    }


    private void Toggle_AmountBar()
    {
        bool playerDetected = _interactable.detection.player != null;
        bool bubbleOn = _interactable.bubble.bubbleOn;

        _foodIcon.Toggle_SubDataBar(playerDetected && !bubbleOn);
    }

    private void Update_Bubble()
    {
        Action_Bubble bubble = _interactable.bubble;

        bubble.Empty_Bubble();
        _interactable.Clear_ActionSubscriptions();

        if (_foodIcon.hasFood == false)
        {
            bubble.Set_Bubble(bubble.setSprites[0], null);

            _interactable.OnAction1Input += Place;
            return;
        }

        if (_foodIcon.DataCount_Maxed())
        {
            bubble.Set_Bubble(bubble.setSprites[1], null);

            _interactable.OnAction1Input += Complete;
            return;
        }

        bubble.Set_Bubble(bubble.setSprites[0], bubble.setSprites[1]);

        _interactable.OnAction1Input += Place;
        _interactable.OnAction2Input += Complete;
    }


    // Functions
    private void Transfer_CurrentFood()
    {
        if (_isComplete) return;

        FoodData_Controller playerIcon = _interactable.detection.player.foodIcon;

        if (_foodIcon.hasFood == false || playerIcon.DataCount_Maxed())
        {
            // swap
            _foodIcon.Swap_Data(playerIcon);
        }
        else
        {
            // player
            playerIcon.Set_CurrentData(_foodIcon.currentData);

            // table
            _foodIcon.Set_CurrentData(null);
        }

        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        _foodIcon.Show_Icon();
        _foodIcon.Toggle_SubDataBar(true);

        Update_Bubble();
        _interactable.UnInteract();
    }


    private bool Place_Available()
    {
        Player_Controller player = _interactable.detection.player;
        if (player == null) return false;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        FoodData_Controller playerIcon = player.foodIcon;
        if (playerIcon.hasFood == false)
        {
            dialog.Update_Dialog(0);
            return false;
        }

        if (_foodIcon.DataCount_Maxed())
        {
            dialog.Update_Dialog(4);
            return false;
        }

        return true;
    }

    private void Place()
    {
        if (Place_Available() == false) return;

        FoodData_Controller playerIcon = _interactable.detection.player.foodIcon;

        // add data to _placedFoods
        _foodIcon.Set_CurrentData(playerIcon.currentData);
        _foodIcon.Show_Icon();
        _foodIcon.Toggle_SubDataBar(true);

        // empty player food
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();

        Update_Bubble();
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(1);
    }


    private void Complete()
    {
        if (foodIcon.hasFood == false) return;
        if (Place_Available()) return;

        _isComplete = true;
        Update_forComplete();

        Update_Bubble();
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(2);
    }
}
