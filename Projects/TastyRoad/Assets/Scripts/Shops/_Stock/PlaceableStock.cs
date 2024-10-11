using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    private List<FoodData> _placedFoods = new();
    public List<FoodData> placedFoods => _placedFoods;

    private bool _isComplete;
    public bool isComplete => _isComplete;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Update_forComplete();
        Update_Bubble();
        Toggle_AmountBar();

        // subscriptions
        _interactable.detection.EnterEvent += Toggle_AmountBar;
        _interactable.detection.ExitEvent += Toggle_AmountBar;

        _interactable.InteractEvent += Update_Bubble;
        _interactable.InteractEvent += Toggle_AmountBar;
        _interactable.UnInteractEvent += Toggle_AmountBar;

        _interactable.Action1Event += Complete;
        _interactable.Action1Event += Place;

        _interactable.Action2Event += Dispose;
    }

    void OnDestroy()
    {
        // subscriptions
        _interactable.detection.EnterEvent -= Toggle_AmountBar;
        _interactable.detection.ExitEvent -= Toggle_AmountBar;

        _interactable.InteractEvent -= Update_Bubble;
        _interactable.InteractEvent -= Toggle_AmountBar;
        _interactable.UnInteractEvent -= Toggle_AmountBar;

        _interactable.Action1Event -= Complete;
        _interactable.Action1Event -= Place;

        _interactable.Action2Event -= Dispose;
    }


    // Updates and Toggles
    public void Update_forComplete() // currently used in test editor
    {
        _foodIcon.Toggle_Height(_isComplete);
        _interactable.bubble.Toggle_Height(_isComplete);

        if (_isComplete == false)
        {
            _sr.sprite = _sprites[0];
            _foodIcon.Toggle_Height(false);

            return;
        }

        _sr.sprite = _sprites[1];
        _foodIcon.Hide_Icon();
    }


    private void Update_AmountBar()
    {
        _foodIcon.currentData.Set_Amount(_placedFoods.Count);
        _foodIcon.Show_AmountBar();

        Toggle_AmountBar();
    }

    private void Toggle_AmountBar()
    {
        Player_Controller playerDetection = _interactable.detection.player;
        FoodData foodData = _foodIcon.currentData;

        if (playerDetection == null || _foodIcon.hasFood == false || foodData.currentAmount <= 0)
        {
            _foodIcon.Toggle_AmountBar(false);
            return;
        }

        _foodIcon.Toggle_BarLock(false);
        _foodIcon.Toggle_AmountBar(!_interactable.bubble.bubbleOn);
    }


    private void Update_Bubble()
    {
        Action_Bubble bubble = _interactable.bubble;

        if (Place_Available())
        {
            bubble.Set_Bubble(bubble.setSprites[0], bubble.setSprites[2]);
            return;
        }

        bubble.Set_Bubble(bubble.setSprites[1], bubble.setSprites[2]);

        if (_foodIcon.hasFood == true && _isComplete == false) return;
        _interactable.UnInteract();
    }


    // Functions
    private bool Place_Available()
    {
        Player_Controller player = _interactable.detection.player;
        if (player == null) return false;

        FoodData_Controller playerIcon = player.foodIcon;
        if (playerIcon.hasFood == false) return false;

        if (_foodIcon.hasFood && _foodIcon.currentData.currentAmount >= _maxAmount) return false;

        return true;
    }

    private void Place()
    {
        if (Place_Available() == false) return;

        FoodData_Controller playerIcon = _interactable.detection.player.foodIcon;

        // show recently placed food
        _foodIcon.Set_CurrentData(new(playerIcon.currentData.foodScrObj));
        _foodIcon.Show_Icon();

        // add data to _placedFoods
        _placedFoods.Add(playerIcon.currentData);

        // empty player food
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();

        //
        Update_AmountBar();
    }


    private void Complete()
    {
        if (foodIcon.hasFood == false) return;
        if (Place_Available()) return;

        _isComplete = true;
        Update_forComplete();
    }


    public void Dispose()
    {
        if (_foodIcon.hasFood == false) return;

        _placedFoods.Clear();

        _isComplete = false;
        Update_forComplete();

        _foodIcon.Set_CurrentData(null);
        _foodIcon.Show_Icon();

        Toggle_AmountBar();
    }
}
