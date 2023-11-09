using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour
{
    //
    [SerializeField] private SpriteRenderer _currentFoodIcon;

    [HideInInspector] public Food currentFood;

    private List<GameObject> _detectedInteractables = new();
    [HideInInspector] public GameObject _closestInteractable;

    //
    private void Update()
    {
        Update_Closest_Interactable();
    }

    private void OnInteract()
    {
        Interact();
    }

    //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IInteractable interactable)) return;

        _detectedInteractables.Add(collision.gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IInteractable interactable)) return;

        _detectedInteractables.Remove(collision.gameObject);
    }

    //
    private GameObject Get_Closest_Interactable()
    {
        float closestDistance = Vector2.Distance(_detectedInteractables[0].transform.position, transform.position);
        GameObject closestInteractable = _detectedInteractables[0];

        for (int i = 0; i < _detectedInteractables.Count; i++)
        {
            if (Vector2.Distance(_detectedInteractables[i].transform.position, transform.position) >= closestDistance) continue;
            closestInteractable = _detectedInteractables[i];
        }

        return closestInteractable;
    }
    private void Update_Closest_Interactable()
    {
        if (_detectedInteractables.Count <= 0) return;
        _closestInteractable = Get_Closest_Interactable();
    }
    private void Interact()
    {
        if (_detectedInteractables.Count <= 0) return;
        if (!_closestInteractable.TryGetComponent(out IInteractable interactable)) return;
        interactable.Interact();
    }

    public void Set_CurrentFood(Food setFood)
    {
        if (setFood == null)
        {
            Empty_CurrentFood();
            return;
        }

        currentFood = setFood;

        _currentFoodIcon.sprite = currentFood.foodScrObj.ingameSprite;
        _currentFoodIcon.color = Color.white;
    }
    public void Empty_CurrentFood()
    {
        currentFood = null;
        _currentFoodIcon.sprite = null;
        _currentFoodIcon.color = Color.clear;
    }
}