using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    private Main_Controller _main;

    [Header("")]
    [SerializeField] private GameObject _foodDrop;
    [SerializeField] private GameObject _collectCard;

    [Header("")]
    [SerializeField] private FoodWeight_Data[] _foodWeights;


    private Coroutine _coroutine;


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }


    // All Drops Control
    private GameObject SnapPosition_Spawn(GameObject spawnItem, Vector2 spawnPosition)
    {
        Vector2 spawnSnapPos = Main_Controller.SnapPosition(spawnPosition);

        if (_main.Position_Claimed(spawnSnapPos)) return null;

        GameObject itemGameObject = Instantiate(spawnItem, spawnSnapPos, Quaternion.identity);
        itemGameObject.transform.SetParent(_main.otherFile);

        return itemGameObject;
    }


    // Food Drop Control
    public FoodDrop Drop_Food(FoodData dropData)
    {
        if (dropData == null) return null;
        if (_coroutine != null) return null;

        GameObject spawnItem = SnapPosition_Spawn(_foodDrop, transform.position);

        if (spawnItem == null) return null;

        FoodDrop foodDrop = spawnItem.GetComponent<FoodDrop>();
        foodDrop.foodIcon.Set_CurrentData(dropData);

        return foodDrop;
    }

    public void Drop_Food(FoodData dropData, int additionalAmount)
    {
        FoodDrop foodDrop = Drop_Food(dropData);

        for (int i = 0; i < additionalAmount; i++)
        {
            foodDrop.foodIcon.Set_CurrentData(dropData);
        }
    }

    public void Drop_Food(List<FoodData> dropDatas)
    {
        if (dropDatas == null) return;
        if (dropDatas.Count <= 0) return;

        if (_coroutine != null) return;

        GameObject spawnItem = SnapPosition_Spawn(_foodDrop, transform.position);

        if (spawnItem == null) return;

        // set drop data
        FoodDrop dropItem = spawnItem.GetComponent<FoodDrop>();
        dropItem.foodIcon.Update_AllDatas(dropDatas);
    }


    /// <returns>
    /// random weighted food from _foodWeights
    /// </returns>
    private Food_ScrObj Weighted_RandomFood()
    {
        // get total wieght
        int totalWeight = 0;

        foreach (FoodWeight_Data data in _foodWeights)
        {
            totalWeight += data.weight;
        }

        // track values
        int randValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // get random food according to weight
        for (int i = 0; i < _foodWeights.Length; i++)
        {
            cumulativeWeight += _foodWeights[i].weight;

            if (randValue >= cumulativeWeight) continue;

            return _foodWeights[i].foodScrObj;
        }

        return null;
    }

    /// <returns>
    /// random weighted food from targetFoods
    /// </returns>
    private Food_ScrObj Weighted_RandomFood(List<FoodWeight_Data> targetFoods)
    {
        // get total wieght
        int totalWeight = 0;

        foreach (var food in targetFoods)
        {
            totalWeight += food.weight;
        }

        // track values
        int randValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // get random food according to weight
        for (int i = 0; i < targetFoods.Count; i++)
        {
            cumulativeWeight += targetFoods[i].weight;

            if (randValue >= cumulativeWeight) continue;

            return targetFoods[i].foodScrObj;
        }

        return null;
    }

    /// <returns>
    /// random weighted food that is higher price than compareFood.
    /// </returns>
    public Food_ScrObj Weighted_RandomFood(Food_ScrObj compareFood)
    {
        List<FoodWeight_Data> targetFoods = new();

        for (int i = 0; i < _foodWeights.Length; i++)
        {
            if (compareFood == _foodWeights[i].foodScrObj) continue;
            if (compareFood.price > _foodWeights[i].foodScrObj.price) continue;

            targetFoods.Add(_foodWeights[i]);
        }

        // return compareFood for highest priced null item
        if (targetFoods.Count <= 0) return compareFood;

        // return all random for highest priced null item
        if (targetFoods.Count <= 0) return Weighted_RandomFood();

        return Weighted_RandomFood(targetFoods);
    }


    // Events
    private void Drop_RandomFood()
    {
        if (_foodWeights.Length <= 0) return;

        Drop_Food(new FoodData(Weighted_RandomFood()));
    }

    public CollectCard Drop_CollectCard()
    {
        if (_coroutine != null) return null;

        GameObject spawnObject = SnapPosition_Spawn(_collectCard, _main.Player().transform.position);

        return spawnObject.GetComponent<CollectCard>();
    }
}

