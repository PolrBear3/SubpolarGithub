using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sr;

    private Game_Controller _gameController;
    private Player_Controller _playerController;

    private Food _currentFood;
    [SerializeField] private FoodState_Indicator _indicator;

    [Header("Sprites")]
    [SerializeField] private Sprite _activeSprite;
    [SerializeField] private Sprite _inactiveSprite;

    private Coroutine _heatCoroutine;

    [Header("Data")]
    [SerializeField] private float _heatIncreaseTime;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();

        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
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
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller playerController)) return;
        _playerController = null;
    }

    // IInteractable
    public void Interact()
    {
        if (_playerController == null) return;
        if (!_playerController.playerInteraction.Is_Closest_Interactable(gameObject)) return;

        Swap_Food();
        Sprite_Update();
        HeatFood_Update();
    }

    // Custom
    private void Sprite_Update()
    {
        // active
        if (_currentFood != null) _sr.sprite = _activeSprite;

        // inactive
        else _sr.sprite = _inactiveSprite;
    }

    private void Swap_Food()
    {
        Player_Interaction player = _playerController.playerInteraction;
        Food playerFood = player.currentFood;

        player.Set_CurrentFood(_currentFood);
        _currentFood = playerFood;
    }

    private IEnumerator Heat_Food()
    {
        while (_currentFood != null)
        {
            yield return new WaitForSeconds(_heatIncreaseTime);

            FoodState_Data stateData = _currentFood.Get_FoodState_Data(FoodState_Type.heated);
            FoodStateIndicator_Data indicatorData = _indicator.Get_Data(FoodState_Type.heated);

            if (indicatorData != null && stateData != null && stateData.stateLevel >= indicatorData.sprite.Count) continue; 

            _currentFood.Update_State(FoodState_Type.heated, 1);
            _indicator.Update_StateSprite(_currentFood.data, FoodState_Type.heated);
        }
    }
    private void HeatFood_Update()
    {
        if (_heatCoroutine != null) StopCoroutine(_heatCoroutine);

        if (_currentFood == null)
        {
            _indicator.gameObject.SetActive(false);
            return;
        }

        _indicator.gameObject.SetActive(true);

        _indicator.Update_StateSprite(_currentFood.data, FoodState_Type.heated);
        _heatCoroutine = StartCoroutine(Heat_Food());
    }
}
