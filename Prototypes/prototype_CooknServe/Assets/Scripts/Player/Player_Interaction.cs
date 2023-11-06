using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour
{
    [HideInInspector] public Player_Controller _playerController;

    //
    private Food _currentFood;
    [SerializeField] private SpriteRenderer _currentFoodIcon;

    public List<GameObject> detectedInteractables = new();

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller playerController)) _playerController = playerController;
    }

    //
    private void OnInteract()
    {
        Interact();
    }

    //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IInteractable interactable)) return;

        detectedInteractables.Add(collision.gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IInteractable interactable)) return;

        detectedInteractables.Remove(collision.gameObject);
    }

    //
    private void Interact()
    {
        if (detectedInteractables.Count <= 0) return;
        if (!detectedInteractables[0].TryGetComponent(out IInteractable interactable)) return;
        interactable.Interact();
    }

    public void Set_CurrentFood(Food setFood)
    {
        _currentFood = setFood;
        _currentFoodIcon.sprite = _currentFood.foodScrObj.ingameSprite;
        _currentFoodIcon.color = Color.white;
    }
    public void Empty_CurrentFood()
    {
        _currentFood = null;
        _currentFoodIcon.sprite = null;
        _currentFoodIcon.color = Color.clear;
    }
}