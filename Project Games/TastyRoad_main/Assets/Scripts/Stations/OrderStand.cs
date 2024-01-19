using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class OrderStand : MonoBehaviour, IInteractable
{
    private SpriteRenderer _spriteRenderer;

    private Main_Controller _mainController;
    private Detection_Controller _detection;
    [SerializeField] private Action_Bubble _actionBubble;

    [Header("Order Stand Sprites")]
    [SerializeField] private Sprite _openStand;
    [SerializeField] private Sprite _closedStand;

    [Header ("NPC Control")]
    [SerializeField] private Transform _lineStartPoint;
    [SerializeField] private Sprite _lineOpenSprite;
    [SerializeField] private Sprite _lineClosedSprite;

    private bool _lineOpen;
    [SerializeField] private float _lineWaitTime;

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

        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _spriteRenderer = sr; }
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }

    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_detection.Has_Player() == true)
        {
            _actionBubble.Toggle_Off();
        }
    }

    // IInteractable
    public void Interact()
    {
        _coinDisplay.SetActive(false);

        _actionBubble.Update_Bubble(LineToggle_Sprite(), _coinSprite);
    }

    // Player Input
    private void OnAction1()
    {
        Line_CurrentNPCs_Toggle();

        _actionBubble.Toggle_Off();
    }

    private void OnAction2()
    {
        Show_CurrentCoin();

        _actionBubble.Toggle_Off();
    }

    // Get Line Toggle Sprite for Action Bubble
    private Sprite LineToggle_Sprite()
    {
        if (_lineOpen == false)
        {
            return _lineOpenSprite;
        }
        else
        {
            return _lineClosedSprite;
        }
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

    // Line Current NPCs
    private void Line_CurrentNPCs_Toggle()
    {
        List<GameObject> characters = _mainController.currentCharacters;

        if (_lineOpen == false)
        {
            _lineOpen = true;
            _spriteRenderer.sprite = _openStand;
        }
        else
        {
            _lineOpen = false;
            _spriteRenderer.sprite = _closedStand;
        }

        // sort closest to farthest
        characters.Sort((customerA, customerB) =>
        Vector2.Distance(customerA.transform.position, transform.position)
        .CompareTo(Vector2.Distance(customerB.transform.position, transform.position)));

        float lineSpaceCount = 0;

        for (int i = 0; i < characters.Count; i++)
        {
            if (!characters[i].TryGetComponent(out NPC_Controller npc)) continue;

            NPC_Movement movement = npc.movement;
            movement.Stop_FreeRoam();

            // line open
            if (_lineOpen == true)
            {
                Vector2 targetPosition = new Vector2(_lineStartPoint.position.x - lineSpaceCount, _lineStartPoint.position.y);
                movement.Assign_TargetPosition(targetPosition, _lineWaitTime);

                lineSpaceCount += 0.75f;
            }
            // line closed
            else
            {
                movement.Free_Roam(0f);
            }
        }
    }
}