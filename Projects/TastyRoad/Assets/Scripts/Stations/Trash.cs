using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour, IInteractable
{
    private Detection_Controller _detection;

    private List<FoodData> _trashedFoodData = new();

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }

    // IInteractable
    public void Interact()
    {
        Trash_Food();
    }

    public void UnInteract()
    {

    }



    // Trash Food
    private void Trash_Food()
    {
        FoodData_Controller playerFoodIcon = _detection.player.foodIcon;

        if (playerFoodIcon.hasFood == true)
        {
            Save_TrashedFood_Data(playerFoodIcon.headData);

            playerFoodIcon.Set_CurrentData(null);
            playerFoodIcon.Show_Icon();
            playerFoodIcon.Show_Condition();
        }
    }



    // Save Trashed Data
    private void Save_TrashedFood_Data(FoodData data)
    {
        _trashedFoodData.Add(data);

        if (_trashedFoodData.Count >= 11)
        {
            _trashedFoodData.RemoveAt(0);
        }
    }
}