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
        foodIcon.AmountBar_Transparency(true);
    }



    // IInteractable
    public new void Interact()
    {
        FoodData_Controller playerIcon = detection.player.foodIcon;
        FoodData playerData = playerIcon.currentFoodData;

        if (playerData.stateData.Count > 0) return;

        if (playerIcon.hasFood == false || playerData.foodScrObj != foodIcon.currentFoodData.foodScrObj)
        {
            Swap_Food();
        }
        else
        {
            Stack_Food();
        }

        if (foodIcon.hasFood == false) return;

        Combine_Food();
    }



    //
    private void Swap_Food()
    {
        FoodData tableData = foodIcon.currentFoodData;
        FoodData_Controller playerIcon = detection.player.foodIcon;

        if (playerIcon.hasFood == false && tableData.currentAmount > 1)
        {
            // give
            foodIcon.Update_Amount(-1);
            foodIcon.Show_AmountBar();

            playerIcon.Assign_Food(tableData.foodScrObj);
        }

        if (tableData.currentAmount > 1)
        {
            foodIcon.Show_AmountBar();
            return;
        }

        // swap
        Basic_SwapFood();
    }

    private void Stack_Food()
    {
        if (foodIcon.currentFoodData.currentAmount >= foodIcon.maxAmount)
        {
            foodIcon.Show_AmountBar();
            return;
        }

        // stack
        foodIcon.Update_Amount(1);
        foodIcon.Show_AmountBar();

        detection.player.foodIcon.Clear_Food();
    }



    //
    private List<FoodData_Controller> SideStation_FoodIcons()
    {
        Main_Controller main = stationController.mainController;
        List<FoodData_Controller> icons = new();

        Vector2 leftPos = new(transform.position.x - 1, transform.position.y);
        Vector2 rightPos = new(transform.position.x + 1, transform.position.y);

        icons.Add(main.Station(leftPos).Food_Icon());
        icons.Add(main.Station(rightPos).Food_Icon());

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

        while (foodIcon.hasFood)
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
            if (foodIcon.currentFoodData.foodScrObj != combinedFood) break;

            // update side food amounts
            for (int i = 0; i < SideStation_FoodIcons().Count; i++)
            {
                SideStation_FoodIcons()[i].Update_Amount(-1);
                SideStation_FoodIcons()[i].Show_AmountBar();
            }

            // combine amount update
            foodIcon.Update_Amount(1);
            foodIcon.Show_AmountBar();

            yield return new WaitForSeconds(_coolTime);
        }
    }
}