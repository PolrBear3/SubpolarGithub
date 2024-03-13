using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combiner : Table, IInteractable
{
    [SerializeField] private float _coolTime;

    private Coroutine _combineCoroutine;



    // UnityEngine
    private void Start()
    {
        stationController.Food_Icon().AmountBar_Transparency(true);
    }



    // IInteractable
    public new void Interact()
    {
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;
        FoodData playerData = playerIcon.currentFoodData;

        if (playerData.stateData.Count > 0) return;

        FoodData_Controller icon = stationController.Food_Icon();

        if (playerIcon.hasFood == false || playerData.foodScrObj != icon.currentFoodData.foodScrObj)
        {
            Swap_Food();
        }
        else
        {
            Stack_Food();
        }

        if (icon.hasFood == false) return;

        Combine_Food();
    }



    //
    private void Swap_Food()
    {
        FoodData_Controller icon = stationController.Food_Icon();
        FoodData tableData = icon.currentFoodData;

        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        if (playerIcon.hasFood == false && tableData.currentAmount > 1)
        {
            // give
            icon.Update_Amount(-1);
            icon.Show_AmountBar();

            playerIcon.Assign_Food(tableData.foodScrObj);
        }

        if (tableData.currentAmount > 1)
        {
            icon.Show_AmountBar();
            return;
        }

        // swap
        Basic_SwapFood();
    }

    private void Stack_Food()
    {
        FoodData_Controller icon = stationController.Food_Icon();

        if (icon.currentFoodData.currentAmount >= icon.maxAmount)
        {
            icon.Show_AmountBar();
            return;
        }

        // stack
        icon.Update_Amount(1);
        icon.Show_AmountBar();

        stationController.detection.player.foodIcon.Clear_Food();
    }



    //
    private List<FoodData_Controller> SideStation_FoodIcons()
    {
        Main_Controller main = stationController.mainController;
        List<FoodData_Controller> icons = new();

        Vector2 leftPos = new(transform.position.x - 1, transform.position.y);
        Vector2 rightPos = new(transform.position.x + 1, transform.position.y);

        if (main.Station(leftPos) != null)
        {
            icons.Add(main.Station(leftPos).Food_Icon());
        }

        if (main.Station(rightPos) != null)
        {
            icons.Add(main.Station(rightPos).Food_Icon());
        }

        for (int i = icons.Count - 1; i >= 0; i--)
        {
            if (icons[i] != null) continue;
            icons.RemoveAt(i);
        }

        return icons;
    }

    private void Combine_Food()
    {
        if (_combineCoroutine != null) StopCoroutine(_combineCoroutine);

        _combineCoroutine = StartCoroutine(Combine_Food_Coroutine());
    }
    private IEnumerator Combine_Food_Coroutine()
    {
        Main_Controller main = stationController.mainController;
        Data_Controller data = main.dataController;

        FoodData_Controller icon = stationController.Food_Icon();

        while (icon.hasFood)
        {
            // check food icons from side stations
            if (SideStation_FoodIcons().Count < 2) break;

            List<Food_ScrObj> ingredients = new();

            // check if has food from side stations
            for (int i = 0; i < SideStation_FoodIcons().Count; i++)
            {
                if (SideStation_FoodIcons()[i].hasFood == false) break;

                ingredients.Add(SideStation_FoodIcons()[i].currentFoodData.foodScrObj);
            }

            Food_ScrObj combinedFood = data.CookedFood(ingredients);

            // check if combined food is archived
            if (main.Is_ArchivedFood(combinedFood) == false) break;

            // check if current food match combine food
            if (icon.currentFoodData.foodScrObj != combinedFood) break;

            // update side food amounts
            for (int i = 0; i < SideStation_FoodIcons().Count; i++)
            {
                SideStation_FoodIcons()[i].Update_Amount(-1);
                SideStation_FoodIcons()[i].Show_AmountBar();
            }

            // combine amount update
            icon.Update_Amount(1);
            icon.Show_AmountBar();

            yield return new WaitForSeconds(_coolTime);
        }
    }
}