using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour
{
    //
    [HideInInspector] public Food currentFood;
    [SerializeField] private Icon_Controller _currentFoodIcon;

    private List<GameObject> _detectedInteractables = new();
    [HideInInspector] public GameObject closestInteractable;

    public FoodState_Indicator indicator;

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
        closestInteractable = null;
    }

    //
    private void Update_Closest_Interactable()
    {
        if (_detectedInteractables.Count <= 0) return;
        closestInteractable = Get_Closest_Interactable();
    }

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
    public bool Is_Closest_Interactable(GameObject interactable)
    {
        if (closestInteractable == interactable) return true;
        return false;
    }

    private void Interact()
    {
        if (_detectedInteractables.Count <= 0) return;

        for (int i = 0; i < _detectedInteractables.Count; i++)
        {
            if (!_detectedInteractables[i].TryGetComponent(out IInteractable interactable)) return;
            interactable.Interact();
        }
    }

    //
    public void Set_CurrentFood(Food setFood)
    {
        if (setFood == null)
        {
            Empty_CurrentFood();
            return;
        }

        currentFood = setFood;
        _currentFoodIcon.Assign(currentFood.foodScrObj.sprite);

        if (currentFood.data.Count <= 0)
        {
            indicator.gameObject.SetActive(false);
            return;
        }

        indicator.gameObject.SetActive(true);
        indicator.Update_StateSprite(currentFood.data);
    }
    public void Empty_CurrentFood()
    {
        currentFood = null;
        _currentFoodIcon.Clear();

        indicator.gameObject.SetActive(false);
    }
}