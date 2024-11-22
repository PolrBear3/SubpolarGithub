using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    private Main_Controller _main;

    [Header("")]
    [SerializeField] private GameObject _dropItem;
    [SerializeField] private GameObject _collectCard;

    [Header("")]
    [SerializeField] private FoodWeight_Data[] _foodWeights;


    public delegate void Event();
    public List<Event> _allDrops = new();

    private Coroutine _coroutine;


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }

    private void Start()
    {
        Set_AllDropTypes();
    }


    // All Drops Control
    private void Set_AllDropTypes()
    {
        _allDrops.Add(Drop_RandomFood);
        _allDrops.Add(Drop_CollectCard);
    }

    public void Drop_Random()
    {
        _allDrops[Random.Range(0, _allDrops.Count)]?.Invoke();
    }


    // Food Drop Control
    public void Drop_Food(FoodData data)
    {
        if (data == null) return;
        if (_coroutine != null) return;

        if (_main.Position_Claimed(transform.position)) return;

        // spawn to nearest snap position
        Vector2 spawnPos = Main_Controller.SnapPosition(transform.position);
        GameObject itemGameObject = Instantiate(_dropItem, spawnPos, Quaternion.identity);
        itemGameObject.transform.SetParent(_main.otherFile);

        // set drop data
        DropItem dropItem = itemGameObject.GetComponent<DropItem>();
        FoodData dropData = new(data);

        dropItem.Set_ItemData(new(dropData));
    }


    // Weighted
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
    /// random weighted food that is higher price than compareFood. Also excludes compareFood.
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

        return Weighted_RandomFood(targetFoods);
    }


    // Events
    private void Drop_RandomFood()
    {
        if (_foodWeights.Length <= 0) return;

        Drop_Food(new(Weighted_RandomFood()));
    }

    public void Drop_CollectCard()
    {
        if (_coroutine != null) return;

        Transform playerTransform = _main.Player().transform;
        Vector2 dropPosition = Main_Controller.SnapPosition(playerTransform.position);

        if (_main.Position_Claimed(dropPosition)) return;

        // spawn to nearest snap position
        GameObject itemGameObject = Instantiate(_collectCard, dropPosition, Quaternion.identity);
        itemGameObject.transform.SetParent(_main.otherFile);
    }
}
