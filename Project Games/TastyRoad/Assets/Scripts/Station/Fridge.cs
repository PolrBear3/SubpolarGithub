using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fridge : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    [SerializeField] private Food_ScrObj _foodScrObj;
    [SerializeField] private List<FoodState_Data> _stateDatas = new();

    [Header("Data")]
    [SerializeField] private int _currentAmount;

    [Header("Food Amount Display")]
    [SerializeField] private SpriteRenderer _foodIcon;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private float _fadeTime;
    private Coroutine _fadeCoroutine;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }
    private void Start()
    {
        _gameController.Connect_Station(gameObject);
        Display_Food();
    }

    // IInteractable
    public void Interact()
    {
        if (!_playerController.playerInteraction.Is_Closest_Interactable(gameObject)) return;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

        Give_Food();
        Display_FoodAmount();
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

    // Display
    private IEnumerator FoodAmount_Hide_Delay()
    {
        yield return new WaitForSeconds(_fadeTime);

        // LeanTween.alpha(_foodIcon.gameObject, 0f, 0.01f);
        _amountText.alpha = 0f;
    }
    private void Display_FoodAmount()
    {
        //_foodIcon.sprite = _foodScrObj.sprite;
        // LeanTween.alpha(_foodIcon.gameObject, 1f, 0.01f);

        _amountText.alpha = 1f;

        _fadeCoroutine = StartCoroutine(FoodAmount_Hide_Delay());
    }

    private void Display_Food()
    {
        _foodIcon.sprite = _foodScrObj.sprite;
    }

    private void Give_Food()
    {
        if (_currentAmount <= 0) return;

        Food newFood = new();
        Food_ScrObj searchedFood = _gameController.dataController.Get_Food(_foodScrObj);

        newFood.Set_Food(searchedFood);

        for (int i = 0; i < _stateDatas.Count; i++)
        {
            newFood.Update_State(_stateDatas[i].stateType, _stateDatas[i].stateLevel);
        }

        _playerController.playerInteraction.Set_CurrentFood(newFood);

        _currentAmount--;
        _amountText.text = _currentAmount.ToString();
    }
}