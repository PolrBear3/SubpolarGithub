using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Order : MonoBehaviour, IInteractable
{
    private BoxCollider2D _bc;

    private Customer_Controller _customerController;
    private Food _orderFood;

    [Header("Default")]
    [SerializeField] private Icon_Controller _currentFoodIcon;
    [SerializeField] private FoodState_Indicator _indicator;
    [SerializeField] private Sprite _coinSprite;

    [Header("Order Menu")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private Icon_Controller _orderIcon;
    [SerializeField] private Timer_Clock _timerClock;

    private bool menuOn;
    private bool _orderServed;
    private bool _calculateComplete;

    private int _calculatePay;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out BoxCollider2D bc)) { _bc = bc; }
        if (gameObject.TryGetComponent(out Customer_Controller customerController)) { _customerController = customerController; }
    }

    // IInteractable
    public void Interact()
    {
        Player_Interaction player = _customerController.playerController.playerInteraction;

        _customerController.customerMovement.Flip_toPlayer();

        // pay complete
        if (_calculateComplete && player.Is_Closest_Interactable(gameObject))
        {
            _customerController.gameController.currentCoin += _calculatePay;
            _customerController.Leave();
            _bc.enabled = false;

            _currentFoodIcon.Clear();
            Reset_Order();

            return;
        }

        if (_orderServed) return;

        // set order and run time
        if (!menuOn && player.Is_Closest_Interactable(gameObject))
        {
            Menu_Activate(true);
            Set_Order();
            return;
        }

        Menu_Activate(false);
    }

    // Player Input
    private void OnOption1()
    {
        if (!menuOn || _customerController.playerController == null || _orderServed) return;

        Menu_Activate(false);
        Pay_Calculation();
        Serve_Order();
    }
    private void OnOption2()
    {
        if (!menuOn || _customerController.playerController == null || _orderServed) return;

        Menu_Activate(false);
        Pay_Calculation();
        Serve_Order();
    }

    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!menuOn) return;
        if (!collision.TryGetComponent(out Player_Controller playerController)) return;
        Menu_Activate(false);
    }

    // Check
    private bool State_isMatch(List<FoodState_Data> playerData)
    {
        List<FoodState_Data> currentData = _orderFood.data;
        int matchCount = currentData.Count;

        if (playerData.Count != currentData.Count) return false;

        for (int i = 0; i < currentData.Count; i++)
        {
            for (int j = 0; j < playerData.Count; j++)
            {
                if (currentData[i].stateType == playerData[j].stateType) matchCount--;
            }
        }

        if (matchCount <= 0) return true;
        return false;
    }
    private bool State_inOrder(List<FoodState_Data> playerData)
    {
        List<FoodState_Data> currentData = _orderFood.data;
        int matchCount = currentData.Count;

        if (playerData.Count != currentData.Count) return false;

        for (int i = 0; i < currentData.Count; i++)
        {
            if (currentData[i].stateType == playerData[i].stateType) matchCount--;
        }

        if (matchCount <= 0) return true;
        return false;
    }
    private bool StateLevel_isMatch(List<FoodState_Data> playerData)
    {
        List<FoodState_Data> currentData = _orderFood.data;
        int matchCount = currentData.Count;

        if (playerData.Count != currentData.Count) return false;

        for (int i = 0; i < currentData.Count; i++)
        {
            for (int j = 0; j < playerData.Count; j++)
            {
                if (currentData[i].stateType != playerData[j].stateType) continue;
                if (currentData[i].stateLevel == playerData[j].stateLevel) matchCount--;
            }
        }

        if (matchCount <= 0) return true;
        return false;
    }

    // Custom
    private void Menu_Activate(bool activate)
    {
        menuOn = activate;

        if (menuOn)
        {
            _menu.SetActive(activate);
            Update_Clock_Position();

            _customerController.customerMovement.Stop_FreeRoam();
        }
        else
        {
            Update_Clock_Position();
            _menu.SetActive(activate);

            _customerController.customerMovement.FreeRoam();
        }
    }
    private void Update_Clock_Position()
    {
        if (menuOn)
        {
            _timerClock.transform.parent = _menu.transform;
            _timerClock.transform.localPosition = new Vector2(-0.5f, 1.487f);
        }
        else
        {
            _timerClock.transform.parent = _customerController.transform;
            _timerClock.transform.localPosition = new Vector2(0f, 0.685f);
        }
    }

    private void Set_Order()
    {
        if (_orderFood != null) return;

        Data_Controller data = _customerController.gameController.dataController;

        // random merged food
        int randFoodID = Random.Range(0, data.mergedFoods.Count);
        Food_ScrObj randFood = data.Get_MergedFood(randFoodID);

        // set order food
        Food orderFood = new();
        orderFood.foodScrObj = randFood;

        _orderFood = orderFood;
        _orderIcon.Assign(_orderFood.foodScrObj.sprite);

        Set_State();

        // run timer
        _timerClock.gameObject.SetActive(true);
        _timerClock.Reset_Time();
        _timerClock.Run_Time();
    }
    private void Set_State()
    {
        List<FoodStateIndicator_Data> data = _indicator.foodStateIndicatorDatas;
        for (int i = 0; i < data.Count; i++)
        {
            if (Random.value > 0.5f) continue;

            int stateLevel = Random.Range(1, 3);
            _orderFood.Update_State(data[i].stateType, stateLevel);
        }

        _orderFood.Shuffle_State();
        _indicator.Update_StateSprite(_orderFood.data);
    }

    private IEnumerator Serve_Animation(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _currentFoodIcon.Assign(_orderFood.foodScrObj.eatSprite);

        yield return new WaitForSeconds(delayTime);
        _currentFoodIcon.Assign(null);

        yield return new WaitForSeconds(delayTime);
        _calculateComplete = true;
        _currentFoodIcon.Assign(_coinSprite);
    }
    private void Serve_Order()
    {
        Player_Interaction player = _customerController.playerController.playerInteraction;

        if (player.currentFood == null) return;

        Food_ScrObj playerFood = player.currentFood.foodScrObj;

        if (playerFood != _orderFood.foodScrObj) return;

        _currentFoodIcon.gameObject.SetActive(true);
        _currentFoodIcon.Assign(_orderFood.foodScrObj.sprite);

        _orderServed = true;
        player.Empty_CurrentFood();

        StartCoroutine(Serve_Animation(1));

        // stop timer
        _timerClock.Stop_Time();
        _timerClock.gameObject.SetActive(false);
    }

    private void Pay_Calculation()
    {
        if (!_calculateComplete) return;

        _calculatePay++;

        if (_timerClock.timeEnd) return;

        Player_Interaction player = _customerController.playerController.playerInteraction;
        if (State_inOrder(player.currentFood.data)) _calculatePay++;
        if (StateLevel_isMatch(player.currentFood.data)) _calculatePay++;
    }
    private void Reset_Order()
    {
        _orderFood = null;
        _orderServed = false;
        _calculateComplete = false;

        _calculatePay = 0;
    }
}
