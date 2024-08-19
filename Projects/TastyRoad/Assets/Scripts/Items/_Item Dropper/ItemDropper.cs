using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] [Range(0, 100)] private int _dropCount;
    private int _currentDropCount;

    [Header("")]
    [SerializeField] private FoodWeight_Data[] _foodWeights;
    [SerializeField] [Range(0, 100)] private int _foodAmountRange;


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


    // Food
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
        dropItem.Set_ItemData(new ItemSlot_Data(dropFood, amount));

        StartCoroutine(Launch_ShowItem(dropItem.gameObject, _defaultLaunchSprite));
    }


    private Food_ScrObj Weighted_RandomFood()
    {
        // get total wieght
        int totalWeight = 0;

        foreach (var food in _foodWeights)
        {
            totalWeight += food.weight;
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


    public void Drop_RandomFood()
    {
        if (_currentDropCount <= 0) return;
        if (_foodWeights.Length <= 0) return;

        _currentDropCount--;

        int randAmount = Random.Range(1, _foodAmountRange);
        Drop_AssignedFood(Weighted_RandomFood(), randAmount);
    }


    //
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
