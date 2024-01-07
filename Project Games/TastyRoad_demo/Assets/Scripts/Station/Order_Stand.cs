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

    [Header("Data")]
    public float lineWaitTime;

    [Header("Current Coin Display")]
    public GameObject coinIcon;
    public TMP_Text amountText;
    public float fadeTime;
    private Coroutine fadeCoroutine;

    [Header("Sprites")]
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    [Header("Options Menu")]
    public GameObject menu;
    public Icon_Controller orderIcon;
    public Sprite _activeIcon;
    public Sprite _inactiveIcon;

    private bool _menuOn;
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

    // InputSystem
    private void OnOption1()
    {
        if (!_menuOn) return;

        Order_Toggle();
        Sprite_Toggle();
        Line_Customers();

        OptionsMenu_Toggle();
    }
    private void OnOption2()
    {
        if (!_menuOn) return;

        Update_CurrentCoin_Text();
        Display_CurrentCoin();

        OptionsMenu_Toggle();
    }

    // IInteractable
    public void Interact()
    {
        if (_menuOn || !_playerController.playerInteraction.Is_Closest_Interactable(gameObject))
        {
            _menuOn = false;
            menu.SetActive(_menuOn);
            return;
        }

        OptionsMenu_Toggle();
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

        _menuOn = false;
        menu.SetActive(_menuOn);
    }

    // Toggles
    private void OptionsMenu_Toggle()
    {
        _menuOn = !_menuOn;
        menu.SetActive(_menuOn);

        if (!_menuOn) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        amountText.alpha = 0f;
        LeanTween.alpha(coinIcon, 0f, 0.01f);
    }
    private void Sprite_Toggle()
    {
        if (_orderOpen)
        {
            _sr.sprite = activeSprite;
            orderIcon.Assign(_inactiveIcon);
        }
        else
        {
            _sr.sprite = inactiveSprite;
            orderIcon.Assign(_activeIcon);
        }
    }
    private void Order_Toggle()
    {
        _orderOpen = !_orderOpen;
    }

    // Coin Text Control
    private IEnumerator CurrentCoin_Hide_Delay()
    {
        yield return new WaitForSeconds(fadeTime);

        amountText.alpha = 0f;
        LeanTween.alpha(coinIcon, 0f, 0.01f);
    }
    private void Display_CurrentCoin()
    {
        amountText.alpha = 1f;
        LeanTween.alpha(coinIcon, 1f, 0.01f);

        fadeCoroutine = StartCoroutine(CurrentCoin_Hide_Delay());
    }

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
                if (allCustomers[i].customerMovement.roamActive) continue;
                allCustomers[i].customerMovement.FreeRoam();
            }

            return;
        }

        allCustomers.Sort((customerA, customerB) =>
            Vector2.Distance(customerA.transform.position, transform.position)
            .CompareTo(Vector2.Distance(customerB.transform.position, transform.position)));

        float lineCountPosition = lineStartPoint.position.x;
        for (int i = 0; i < allCustomers.Count; i++)
        {
            if (allCustomers[i].customerOrder.orderFood != null) continue;

            Vector2 linePosition = new Vector2(lineStartPoint.transform.position.x + lineCountPosition, lineStartPoint.transform.position.y);

            allCustomers[i].customerMovement.Stop_FreeRoam();
            allCustomers[i].customerMovement.Update_NextPosition(linePosition);

            lineCountPosition -= .75f;

            allCustomers[i].customerMovement.Stop_FreeRoam(lineWaitTime);
        }
    }
}
