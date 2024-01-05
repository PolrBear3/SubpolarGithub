using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice_Table : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;

    private Player_Controller _playerController;

    private Food _currentFood;
    [SerializeField] private SpriteRenderer _currentFoodIcon;
    [SerializeField] private SpriteRenderer _sliceIcon;
    [SerializeField] private FoodState_Indicator _indicator;

    [Header("Data")]
    private Coroutine _sliceCoroutine;
    [SerializeField] private float _sliceIncreaseTime;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }
    private void Start()
    {
        _gameController.Connect_Station(gameObject);
    }

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller playerController)) return;
        _playerController = playerController;

        if (_sliceCoroutine == null) return;
        SliceFood_Update();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller playerController)) return;
        _playerController = null;

        if (_sliceCoroutine == null) return;
        StopCoroutine(_sliceCoroutine);

        _sliceIcon.color = Color.clear;
        _indicator.transform.localPosition = new Vector2(0, 1f);
    }

    // IInteractable
    public void Interact()
    {
        if (_playerController == null) return;
        if (!_playerController.playerInteraction.Is_Closest_Interactable(gameObject)) return;

        Swap_Food();
        CurrentFood_Sprite_Update();
        SliceFood_Update();
    }

    // Custom
    private void CurrentFood_Sprite_Update()
    {
        if (_currentFood == null)
        {
            _currentFoodIcon.sprite = null;
            return;
        }

        _currentFoodIcon.sprite = _currentFood.foodScrObj.sprite;
    }

    private void Swap_Food()
    {
        Player_Interaction player = _playerController.playerInteraction;
        Food playerFood = player.currentFood;

        player.Set_CurrentFood(_currentFood);
        _currentFood = playerFood;
    }

    private IEnumerator Slice_Food()
    {
        while (_currentFood != null)
        {
            yield return new WaitForSeconds(_sliceIncreaseTime);

            FoodState_Data stateData = _currentFood.Get_FoodState_Data(FoodState_Type.sliced);
            FoodStateIndicator_Data indicatorData = _indicator.Get_Data(FoodState_Type.sliced);

            if (stateData != null && stateData.stateLevel >= indicatorData.sprite.Count)
            {
                _sliceIcon.color = Color.clear;
                _indicator.transform.localPosition = new Vector2(0, 1f);

                continue;
            }

            _currentFood.Update_State(FoodState_Type.sliced, 1);
            _indicator.Update_StateSprite(_currentFood.data, FoodState_Type.sliced);
        }
    }
    private void SliceFood_Update()
    {
        if (_sliceCoroutine != null) StopCoroutine(_sliceCoroutine);

        if (_currentFood == null)
        {
            _indicator.gameObject.SetActive(false);
            _sliceIcon.color = Color.clear;
            _indicator.transform.localPosition = new Vector2(0, 1f);

            return;
        }

        _indicator.gameObject.SetActive(true);
        _indicator.Update_StateSprite(_currentFood.data, FoodState_Type.sliced);

        _sliceCoroutine = StartCoroutine(Slice_Food());

        FoodState_Data stateData = _currentFood.Get_FoodState_Data(FoodState_Type.sliced);
        FoodStateIndicator_Data indicatorData = _indicator.Get_Data(FoodState_Type.sliced);

        if (stateData != null && stateData.stateLevel >= indicatorData.sprite.Count)
        {
            _sliceIcon.color = Color.clear;
            _indicator.transform.localPosition = new Vector2(0, 1f);

            return;
        }

        _indicator.transform.localPosition = new Vector2(0, 1.44f);
        _sliceIcon.color = Color.white;
    }
}
