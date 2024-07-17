using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCard : MonoBehaviour
{
    [SerializeField] private ActionBubble_Interactable _interactable;


    // UnityEngine
    private void Start()
    {
        _interactable.Action1Event += FoodIngredient_toArchive;
    }

    private void OnDestroy()
    {
        _interactable.Action1Event -= FoodIngredient_toArchive;
    }


    // Set



    // Functions
    private void FoodIngredient_toArchive()
    {
        Debug.Log("FoodIngredient_toArchive");
    }
}
