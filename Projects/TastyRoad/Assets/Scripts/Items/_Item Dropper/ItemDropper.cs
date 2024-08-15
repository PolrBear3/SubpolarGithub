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
    [SerializeField] private Food_ScrObj[] _foodDrops;
    [SerializeField] private int[] _foodDropWeights;
    [SerializeField] [Range(1, 100)] private int _foodAmountRange;

    [Header("")]
    [SerializeField] [Range(1, 100)] private int _nuggetAmountRange;


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


    // Launch
    private IEnumerator Launch_ShowItem(SpriteRenderer dropItemSR, Sprite launchSprite)
    {
        // hide
        dropItemSR.color = Color.clear;

        // launch
        Transform playerTransform = _main.Player().transform;
        GameObject launchCoin = _launcher.Parabola_CoinLaunch(launchSprite, playerTransform.position).gameObject;

        // show
        while (launchCoin != null) yield return null;
        dropItemSR.color = Color.white;

        _coroutine = null;
        yield break;
    }


    // All Drops Control
    private void Set_AllDropTypes()
    {
        _allDrops.Add(Drop_RandomFood);
        _allDrops.Add(Drop_CollectCard);
        _allDrops.Add(Drop_GoldenNuggets);
    }

    //
    public void Drop_Random()
    {
        _allDrops[Random.Range(0, _allDrops.Count)]?.Invoke();
    }


    // Food
    public void Drop_AssignedFood(Food_ScrObj dropFood, int amount)
    {
        if (_coroutine != null) return;

        Transform playerTransform = _main.Player().transform;
        Vector2 dropPosition = Main_Controller.SnapPosition(playerTransform.position);

        if (_main.Position_Claimed(dropPosition)) return;

        // spawn to nearest snap position
        GameObject itemGameObject = Instantiate(_dropItem, dropPosition, Quaternion.identity);
        itemGameObject.transform.SetParent(_main.otherFile);

        // set drop data
        DropItem dropItem = itemGameObject.GetComponent<DropItem>();
        dropItem.Set_ItemData(new ItemSlot_Data(dropFood, amount));

        StartCoroutine(Launch_ShowItem(dropItem.sr, _defaultLaunchSprite));
    }


    private Food_ScrObj Weighted_RandomFood()
    {
        // get total wieght
        int totalWeight = 0;

        foreach (var weight in _foodDropWeights)
        {
            totalWeight += weight;
        }

        // track values
        int randValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // get random food according to weight
        for (int i = 0; i < _foodDropWeights.Length; i++)
        {
            cumulativeWeight += _foodDropWeights[i];

            if (randValue >= cumulativeWeight) continue;

            return _foodDrops[i];
        }

        return null;
    }

    public void Drop_RandomFood()
    {
        if (_foodDrops.Length <= 0) return;

        int randAmount = Random.Range(0, _foodAmountRange);

        Drop_AssignedFood(Weighted_RandomFood(), randAmount);
    }


    //
    private void Drop_GoldenNuggets()
    {
        int randAmount = Random.Range(0, _nuggetAmountRange);
        Drop_AssignedFood(_main.dataController.goldenNugget, randAmount);
    }


    //
    public void Drop_CollectCard()
    {
        if (_coroutine != null) return;

        Transform playerTransform = _main.Player().transform;
        Vector2 dropPosition = Main_Controller.SnapPosition(playerTransform.position);

        if (_main.Position_Claimed(dropPosition)) return;

        // spawn to nearest snap position
        GameObject itemGameObject = Instantiate(_collectCard, dropPosition, Quaternion.identity);
        CollectCard dropCard = itemGameObject.GetComponent<CollectCard>();
        itemGameObject.transform.SetParent(_main.otherFile);

        StartCoroutine(Launch_ShowItem(dropCard.sr, dropCard.launchSprite));
    }
}
