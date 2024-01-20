using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceTable : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;

    private Detection_Controller _detection;

    [SerializeField] private FoodData_Controller _foodIcon;
    [SerializeField] private StateAnimationIcon_Controller _stateAnimation;

    private Coroutine _sliceCoroutine;
    [SerializeField] private float _sliceIncreaseTime;

    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentStation(gameObject);

        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_detection.Has_Player() == true)
        {
            Slice_Food();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_detection.Has_Player() == false)
        {
            Slice_Food();
        }
    }

    // IInteractable
    public void Interact()
    {
        Swap_Food();
        Slice_Food();
    }

    // Swap SliceTable and Player Food
    private void Swap_Food()
    {
        FoodData_Controller playerIcon = _detection.player.foodIcon;

        Food_ScrObj ovenFood = _foodIcon.currentFoodData.foodScrObj;
        List<FoodState_Data> ovenStateData = new(_foodIcon.currentFoodData.stateData);

        Food_ScrObj playerFood = playerIcon.currentFoodData.foodScrObj;
        List<FoodState_Data> playerStateData = new(playerIcon.currentFoodData.stateData);

        _foodIcon.Assign_Food(playerFood);
        _foodIcon.Assign_State(playerStateData);

        playerIcon.Assign_Food(ovenFood);
        playerIcon.Assign_State(ovenStateData);
    }

    // Slice Food
    private IEnumerator Slice_Food_Coroutine()
    {
        while (_foodIcon.hasFood == true && _detection.Has_Player() == true)
        {
            _stateAnimation.Toggle_Transparency(true);
            _stateAnimation.Assign_Animation(FoodState_Type.sliced);

            yield return new WaitForSeconds(_sliceIncreaseTime);

            _foodIcon.Update_State(FoodState_Type.sliced, 1);
        }
    }
    private void Slice_Food()
    {
        _stateAnimation.Toggle_Transparency(false);

        if (_sliceCoroutine != null)
        {
            StopCoroutine(_sliceCoroutine);
        }

        _sliceCoroutine = StartCoroutine(Slice_Food_Coroutine());
    }
}
