using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Action_Bubble : MonoBehaviour
{
    private PlayerInput _playerInput;
    [SerializeField] private GameObject _toggle;

    [SerializeField] private GameObject _leftBubble;
    [SerializeField] private GameObject _rightBubble;

    private SpriteRenderer _leftIcon;
    private SpriteRenderer _rightIcon;

    // UnityEngine
    private void Awake()
    {
        if (_leftBubble.transform.GetChild(0).TryGetComponent(out SpriteRenderer leftIcon)) { _leftIcon = leftIcon; }
        if (_rightBubble.transform.GetChild(0).TryGetComponent(out SpriteRenderer rightIcon)) { _rightIcon = rightIcon; }
    }

    private void Start()
    {
        Toggle_Off();
    }

    // Set Player Input
    public void Set_PlayerInput(PlayerInput playerInput)
    {
        _playerInput = playerInput;
    }

    // Hide Action Bubble
    public void Toggle_Off()
    {
        _toggle.SetActive(false);
        _playerInput.enabled = false;
    }

    // Update Bubble Icon Sprite
    public void Update_Bubble(Sprite leftIcon, Sprite rightIcon)
    {
        // turn off if bubble is alreay || no sprites are assigned
        if (_toggle.activeSelf == true || leftIcon == null)
        {
            Toggle_Off();
            return;
        }

        // toggle on
        _toggle.SetActive(true);
        _playerInput.enabled = true;

        // left bubble toggle on
        _leftBubble.SetActive(true);
        _leftIcon.sprite = leftIcon;

        // right bubble toggle
        if (rightIcon != null)
        {
            _rightBubble.SetActive(true);
            _rightIcon.sprite = rightIcon;
        }
        else
        {
            _rightBubble.SetActive(false);
        }

        Update_BubblePosition();
    }

    // Update Bubble Position
    private void Update_BubblePosition()
    {
        if (_rightIcon.sprite == null)
        {
            _leftBubble.transform.localPosition = new Vector2(0f, 0.56f);
        }
        else
        {
            _leftBubble.transform.localPosition = new Vector2(-0.4f, 0.56f);
            _rightBubble.transform.localPosition = new Vector2(0.4f, 0.56f);
        }
    }
}