using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private ActionBubble_Interactable _interactable;
    public ActionBubble_Interactable interactable => _interactable;

    [Header("")]
    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [SerializeField] private FoodData_Controller _previewIcon;
    public FoodData_Controller previewIcon => _previewIcon;

    [Header("")]
    [SerializeField] private Sprite[] _sprites;

    [Header("")]
    [SerializeField][Range(1, 98)] private int _maxAmount;


    private bool _isComplete;
    public bool isComplete => _isComplete;
    
    
    [Space(60)]
    [SerializeField] private Guide_ScrObj _guideScrObj;


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

        _interactable.OnInteract += () => VideoGuide_Controller.instance.Trigger_Guide(_guideScrObj);
        
        _interactable.OnInteract += Toggle_AmountBar;
        _interactable.OnUnInteract += Toggle_AmountBar;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.detection.EnterEvent -= Toggle_AmountBar;
        _interactable.detection.ExitEvent -= Toggle_AmountBar;

        _interactable.OnInteract -= Toggle_AmountBar;
        _interactable.OnUnInteract -= Toggle_AmountBar;

        _interactable.OnAction1 -= Complete;
        _interactable.OnAction1 -= Place;
    }


    // Data Control
    public void Load_Data(List<FoodData> placedData, bool completeData)
    {
        if (placedData == null || placedData.Count <= 0) return;

        _foodIcon.Update_AllDatas(placedData);
        _isComplete = completeData;
    }

    public void Reset_Data()
    {
        _foodIcon.Update_AllDatas(null);
        _foodIcon.Show_Icon();

        _isComplete = false;
        Update_forComplete();

        _interactable.LockInteract(false);

        Update_Bubble();
        Toggle_AmountBar();
    }


    // Updates and Toggles
    public void Update_forComplete()
    {
        _foodIcon.Toggle_Height(_isComplete);
        _interactable.bubble.Toggle_Height(_isComplete);

        _foodIcon.Hide_Condition();

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

        bool foodPlaced = _foodIcon.hasFood;

        _foodIcon.Toggle_SubDataBar(foodPlaced && playerDetected && !bubbleOn);
        _previewIcon.amountBar.Toggle(!foodPlaced && playerDetected && !bubbleOn);
    }

    private void Update_Bubble()
    {
        Action_Bubble bubble = _interactable.bubble;

        bubble.Empty_Bubble();
        _interactable.Clear_ActionSubscriptions();

        if (_foodIcon.hasFood == false)
        {
            bubble.Set_Bubble(bubble.setSprites[0], null);

            _interactable.OnAction1 += Place;
            return;
        }

        if (_foodIcon.DataCount_Maxed())
        {
            bubble.Set_Bubble(bubble.setSprites[1], null);

            _interactable.OnAction1 += Complete;
            return;
        }

        bubble.Set_Bubble(bubble.setSprites[0], bubble.setSprites[1]);

        _interactable.OnAction1 += Place;
        _interactable.OnAction2 += Complete;
    }


    // Functions
    private bool Place_Available()
    {
        Player_Controller player = _interactable.detection.player;

        if (player == null) return false;

        FoodData_Controller playerIcon = player.foodIcon;

        if (playerIcon.hasFood == false) return false;

        if (_foodIcon.DataCount_Maxed())
        {
            gameObject.GetComponent<DialogTrigger>().Update_Dialog(1);
            return false;
        }

        return true;
    }

    private void Place()
    {
        if (Place_Available() == false) return;
        if (!_foodIcon.hasFood) _foodIcon.Update_AllDatas(null);

        _previewIcon.Update_AllDatas(null);
        _previewIcon.Show_Icon();

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
    }


    private void Complete()
    {
        if (!foodIcon.hasFood) return;

        _isComplete = true;
        Update_forComplete();

        _interactable.LockInteract(true);

        Update_Bubble();
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(0);
    }
}
