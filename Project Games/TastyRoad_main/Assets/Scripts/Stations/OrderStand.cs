using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class OrderStand : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;

    [SerializeField] private Action_Bubble _actionBubble;

    [Header ("Current Coin Display")]
    [SerializeField] private GameObject _coinDisplay;
    [SerializeField] private TextMeshPro _coinText;
    [SerializeField] private Sprite _coinSprite;

    private Coroutine coinCoroutine;
    [SerializeField] private float _displayTime;

    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentStation(gameObject);

        if (gameObject.TryGetComponent(out PlayerInput playerInput))
        {
            _actionBubble.Set_PlayerInput(playerInput);
        }
    }

    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player_Controller player))
        {
            _actionBubble.Toggle_Off();
        }
    }

    // IInteractable
    public void Interact()
    {
        _coinDisplay.SetActive(false);

        _actionBubble.Update_Bubble(_coinSprite, null);
    }

    // Player Input
    private void OnAction1()
    {
        Show_CurrentCoin();

        _actionBubble.Toggle_Off();
    }

    private void OnAction2()
    {
        // Line_NPCs();

        _actionBubble.Toggle_Off();
    }

    // Show Current Coin
    private IEnumerator Show_CurrentCoin_Coroutine()
    {
        _coinDisplay.SetActive(true);
        _coinText.text = _mainController.currentCoin.ToString();

        yield return new WaitForSeconds(_displayTime);

        _coinDisplay.SetActive(false);
    }
    private void Show_CurrentCoin()
    {
        if (coinCoroutine != null)
        {
            StopCoroutine(coinCoroutine);
        }

        coinCoroutine = StartCoroutine(Show_CurrentCoin_Coroutine());
    }
}