using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    private Main_Controller _main;

    [Header("")]
    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private GameObject _dropItem;
    [SerializeField] private GameObject _collectCard;

    [Header("")]
    [SerializeField] private Sprite _defaultLaunchSprite;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _dropAmountRange;

    [SerializeField][Range(0, 100)] private int _dropCount;
    private int _currentDropCount;

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

        if (_dropCount <= 0) return;
        Set_DropCount(_dropCount);
    }


    // Drop Count
    public void Set_DropCount(int setCount)
    {
        _currentDropCount = setCount;
    }

    public int Random_DropAmount()
    {
        return Random.Range(1, _dropAmountRange);
    }


    // Launch
    private IEnumerator Launch_ShowItem(GameObject dropItem, Sprite launchSprite)
    {
        // deactivate
        dropItem.SetActive(false);

        // launch
        Transform playerTransform = _main.Player().transform;
        GameObject launchCoin = _launcher.Parabola_CoinLaunch(launchSprite, playerTransform.position).gameObject;

        // activate when item drops
        while (launchCoin != null) yield return null;
        dropItem.SetActive(true);

        _coroutine = null;
        yield break;
    }


    // All Drops Control
    private void Set_AllDropTypes()
    {
        _allDrops.Add(Drop_RandomFood);
        _allDrops.Add(Drop_CollectCard);
    }

    //
    public void Drop_Random()
    {
        _allDrops[Random.Range(0, _allDrops.Count)]?.Invoke();
    }


    // Food Drop Control
    public void Drop_AssignedFood(Food_ScrObj dropFood, int amount)
    {
        if (_currentDropCount <= 0) return;
        if (_coroutine != null) return;

        Transform playerTransform = _main.Player().transform;
        Vector2 dropPosition = Main_Controller.SnapPosition(playerTransform.position);

        if (_main.Position_Claimed(dropPosition)) return;

        _currentDropCount--;

        // spawn to nearest snap position
        GameObject itemGameObject = Instantiate(_dropItem, dropPosition, Quaternion.identity);
        itemGameObject.transform.SetParent(_main.otherFile);

        // set drop data
        DropItem dropItem = itemGameObject.GetComponent<DropItem>();

        FoodData dropData = new(dropFood, amount);
        dropItem.Set_ItemData(new(dropData));

        StartCoroutine(Launch_ShowItem(dropItem.gameObject, _defaultLaunchSprite));
    }


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


    private void FisherYates_FoodShuffle()
    {
        for (int i = _foodWeights.Length - 1; i >= 0; i--)
        {
            int randIndex = Random.Range(0, i + 1);
            FoodWeight_Data randData = new(_foodWeights[randIndex].foodScrObj, _foodWeights[randIndex].weight);

            _foodWeights[randIndex] = _foodWeights[i];
            _foodWeights[i] = randData;
        }
    }


    // Events
    private void Drop_RandomFood()
    {
        if (_currentDropCount <= 0) return;
        if (_foodWeights.Length <= 0) return;

        _currentDropCount--;

        int randAmount = Random.Range(1, _dropAmountRange);
        Drop_AssignedFood(Weighted_RandomFood(), randAmount);
    }

    public void Drop_CollectCard()
    {
        if (_currentDropCount <= 0) return;
        if (_coroutine != null) return;

        Transform playerTransform = _main.Player().transform;
        Vector2 dropPosition = Main_Controller.SnapPosition(playerTransform.position);

        if (_main.Position_Claimed(dropPosition)) return;

        _currentDropCount--;

        // spawn to nearest snap position
        GameObject itemGameObject = Instantiate(_collectCard, dropPosition, Quaternion.identity);
        CollectCard dropCard = itemGameObject.GetComponent<CollectCard>();
        itemGameObject.transform.SetParent(_main.otherFile);

        StartCoroutine(Launch_ShowItem(dropCard.gameObject, dropCard.launchSprite));
    }
}
