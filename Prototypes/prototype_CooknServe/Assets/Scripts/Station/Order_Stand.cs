using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Order_Stand : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sr;

    private Game_Controller _gameController;
    private Player_Controller _playerController;

    public Transform lineStartPoint;
    public TMP_Text amountText;

    [Header("Sprites")]
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private bool _orderOpen;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();

        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
    }
    private void Start()
    {
        _gameController.Connect_Station(gameObject);
        Update_CurrentCoin_Text();
    }

    // IInteractable
    public void Interact()
    {
        Order_Toggle();
        Sprite_Toggle();
        Line_Customers();
    }

    // OnTriggers
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

    // Toggles
    private void Sprite_Toggle()
    {
        if (_orderOpen) _sr.sprite = activeSprite;
        else _sr.sprite = inactiveSprite;
    }
    private void Order_Toggle()
    {
        _orderOpen = !_orderOpen;
    }

    // Coin Text Control
    private void Update_CurrentCoin_Text()
    {
        amountText.text = _gameController.currentCoin.ToString();
    }

    // Functions
    private void Line_Customers()
    {
        List<GameObject> currentCharacters = _gameController.currentCharacters;
        List<Customer_Controller> allCustomers = new();

        for (int i = 0; i < currentCharacters.Count; i++)
        {
            if (!currentCharacters[i].TryGetComponent(out Customer_Controller customer)) continue;
            allCustomers.Add(customer);
        }

        if (!_orderOpen)
        {
            for (int i = 0; i < allCustomers.Count; i++)
            {
                allCustomers[i].customerMovement.Start_FreeRoam();
            }

            return;
        }

        allCustomers.Sort((customerA, customerB) =>
            Vector2.Distance(customerA.transform.position, transform.position)
            .CompareTo(Vector2.Distance(customerB.transform.position, transform.position)));

        float lineCountPosition = lineStartPoint.position.x;
        for (int i = 0; i < allCustomers.Count; i++)
        {
            Vector2 linePosition = new Vector2(lineStartPoint.transform.position.x + lineCountPosition, lineStartPoint.transform.position.y);

            allCustomers[i].customerMovement.Stop_FreeRoam();
            allCustomers[i].customerMovement.Update_NextPosition(linePosition);

            lineCountPosition -= .75f;
        }
    }
}
