using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Action_Bubble : MonoBehaviour
{
    [SerializeField] private GameObject _toggle;
    [SerializeField] private GameObject _leftBubble;
    [SerializeField] private GameObject _rightBubble;

    private SpriteRenderer _leftIcon;
    private SpriteRenderer _rightIcon;

    private bool _bubbleOn;
    public bool bubbleOn => _bubbleOn;

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

    // Hide Action Bubble
    public void Toggle_Off()
    {
        _bubbleOn = false;
        _toggle.SetActive(false);
    }

    // Update Bubble Icon Sprite
    public void Update_Bubble(Sprite leftIcon, Sprite rightIcon)
    {
        // turn off if bubble is on || no sprites are assigned
        if (_toggle.activeSelf == true || leftIcon == null)
        {
            Toggle_Off();
            return;
        }

        // toggle on
        _bubbleOn = true;

        _toggle.SetActive(true);

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
    public void Update_Bubble(Food_ScrObj leftFood, Food_ScrObj rightFood)
    {
        // reposition left food icon to the center of the bubble
        _leftIcon.transform.localPosition = leftFood.centerPosition / 100;

        // reposition right food icon to the center of the bubble
        Sprite rightFoodSprite = null;

        if (rightFood != null)
        {
            rightFoodSprite = rightFood.sprite;
            _rightIcon.transform.localPosition = rightFood.centerPosition / 100;
        }

        //
        Update_Bubble(leftFood.sprite, rightFoodSprite);
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