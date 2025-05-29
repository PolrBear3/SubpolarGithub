using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private GameObject _foodDrop;
    [SerializeField] private GameObject _collectCard;
    [SerializeField] private GameObject _goldDrop;


    [Space(20)]
    [SerializeField] private FoodWeight_Data[] _foodWeights;
    public FoodWeight_Data[] foodWeights => _foodWeights;

    [SerializeField] private StationWeight_Data[] _stationWeights;
    public StationWeight_Data[] stationWeights => _stationWeights;

    [SerializeField] private Vector2 _goldAmountRange;


    private Vector2 _dropPosition;

    private Coroutine _coroutine;


    // All Drops Control
    public void Set_DropPosition(Vector2 setPos)
    {
        _dropPosition = setPos;
    }

    private Vector2 DropPosition()
    {
        if (_dropPosition == Vector2.zero) return transform.position;
        return _dropPosition;
    }


    private GameObject SnapPosition_Spawn(GameObject spawnItem, Vector2 spawnPosition)
    {
        Main_Controller main = Main_Controller.instance;
        Vector2 spawnSnapPos = main.SnapPosition(spawnPosition);

        if (main.Position_Claimed(spawnSnapPos)) return null;

        GameObject itemGameObject = Instantiate(spawnItem, spawnSnapPos, Quaternion.identity);
        itemGameObject.transform.SetParent(main.otherFile);

        return itemGameObject;
    }


    // Food Drop Control
    public FoodDrop Drop_Food(Vector2 spawnPosition)
    {
        GameObject dropObject = SnapPosition_Spawn(_foodDrop, spawnPosition);

        if (dropObject == null) return null;

        return dropObject.GetComponent<FoodDrop>();
    }

    public FoodDrop Drop_Food(FoodData dropData)
    {
        if (dropData == null) return null;
        if (_coroutine != null) return null;

        GameObject spawnItem = SnapPosition_Spawn(_foodDrop, DropPosition());

        if (spawnItem == null) return null;

        FoodDrop foodDrop = spawnItem.GetComponent<FoodDrop>();
        foodDrop.foodIcon.Set_CurrentData(dropData);

        return foodDrop;
    }

    public FoodDrop Drop_Food(FoodData dropData, int additionalAmount)
    {
        FoodDrop foodDrop = Drop_Food(dropData);

        if (additionalAmount <= 0) return foodDrop;

        for (int i = 0; i < additionalAmount; i++)
        {
            foodDrop.foodIcon.Set_CurrentData(dropData);
        }

        return foodDrop;
    }

    public FoodDrop Drop_Food(List<FoodData> dropDatas)
    {
        if (dropDatas == null) return null;
        if (dropDatas.Count <= 0) return null;

        if (_coroutine != null) return null;

        GameObject spawnItem = SnapPosition_Spawn(_foodDrop, DropPosition());

        if (spawnItem == null) return null;

        // set drop data
        FoodDrop dropItem = spawnItem.GetComponent<FoodDrop>();
        dropItem.foodIcon.Update_AllDatas(dropDatas);

        return dropItem;
    }


    /// <returns>
    /// random weighted food from _foodWeights
    /// </returns>
    public Food_ScrObj Weighted_RandomFood()
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


    // Collect Card Drop Control
    private Station_ScrObj Weighted_RandomStation()
    {
        // get total wieght
        int totalWeight = 0;

        foreach (StationWeight_Data data in _stationWeights)
        {
            totalWeight += data.weight;
        }

        // track values
        int randValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // get random food according to weight
        for (int i = 0; i < _stationWeights.Length; i++)
        {
            cumulativeWeight += _stationWeights[i].weight;

            if (randValue >= cumulativeWeight) continue;

            return _stationWeights[i].stationScrObj;
        }

        return null;
    }


    public void Drop_CollectCard()
    {
        if (_coroutine != null) return;

        SnapPosition_Spawn(_collectCard, DropPosition());
    }

    public CollectCard DropReturn_CollectCard()
    {
        if (_coroutine != null) return null;

        GameObject spawnObject = SnapPosition_Spawn(_collectCard, DropPosition());
        return spawnObject.GetComponent<CollectCard>();
    }


    public void Drop_Ingredient()
    {
        CollectCard droppedCard = DropReturn_CollectCard();

        droppedCard.Set_FoodIngredient(Weighted_RandomFood());
        droppedCard.Assign_Pickup(droppedCard.FoodIngredient_toArchive);
    }

    public void Drop_StationBluePrint()
    {
        CollectCard droppedCard = DropReturn_CollectCard();

        droppedCard.Set_Blueprint(Weighted_RandomStation());
        droppedCard.Assign_Pickup(droppedCard.StationBluePrint_toArchive);
    }
    
    
    // Gold Drop
    public GoldDrop Dropped_Gold()
    {
        int dropAmount = Random.Range((int)_goldAmountRange.x, (int)_goldAmountRange.y + 1);
        
        GameObject spawnGold = SnapPosition_Spawn(_goldDrop, DropPosition());
        GoldDrop goldDrop = spawnGold.GetComponent<GoldDrop>();
        
        goldDrop.Set_Data(dropAmount);
        return goldDrop;
    }

    public void Drop_Gold()
    {
        Dropped_Gold();
    }
}

