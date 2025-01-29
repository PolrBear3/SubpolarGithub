using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour, ISaveLoadable
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private IInteractable_Controller _iInteractable;
    [SerializeField] private ItemDropper _itemDropper;
    [SerializeField] private Custom_PositionClaimer _positionClaimer;

    [Header("")]
    [SerializeField] private Sprite[] _toggleSprites;

    [Header("")]
    [SerializeField] private FoodData[] _lootDatas;


    private bool _itemDropped;
    private FoodDrop _droppedItem;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Load_Data();
        Update_CurrentSprite();

        // subscriptions
        WorldMap_Controller.OnNewLocation += Reset_Data;

        _iInteractable.OnInteract += Drop_LootBoxItem;
    }

    private void OnDestroy()
    {
        // subscriptions
        WorldMap_Controller.OnNewLocation -= Reset_Data;

        _iInteractable.OnInteract -= Drop_LootBoxItem;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("LootBox/_itemDropped", _itemDropped);

        List<FoodData> droppedData = new();
        if (_droppedItem != null) droppedData = _droppedItem.foodIcon.AllDatas();

        ES3.Save("LootBox/_droppedData", droppedData);
    }

    public void Load_Data()
    {
        _itemDropped = ES3.Load("LootBox/_itemDropped", _itemDropped);

        List<FoodData> droppedData = new();
        droppedData = ES3.Load("LootBox/_droppedData", droppedData);

        Drop_LootBoxItem(droppedData);
    }


    private void Reset_Data()
    {
        _itemDropped = false;

        if (_droppedItem == null) return;

        GameObject droppedItem = _droppedItem.gameObject;
        _droppedItem = null;

        Destroy(droppedItem);
    }


    // Indication
    private void Update_CurrentSprite()
    {
        if (_itemDropped == false)
        {
            _sr.sprite = _toggleSprites[0];
            return;
        }

        _sr.sprite = _toggleSprites[1];
    }


    // Control
    private Vector2 Drop_SpawnPoint()
    {
        Main_Controller main = _iInteractable.mainController;
        Location_Controller currentLocation = main.currentLocation;

        List<Vector2> surroundingPositions = _positionClaimer.All_InteractPositions();

        for (int i = 0; i < surroundingPositions.Count; i++)
        {
            if (currentLocation.Restricted_Position(surroundingPositions[i])) continue;
            return surroundingPositions[i];
        }

        return transform.position;
    }


    private void Drop_LootBoxItem()
    {
        if (_itemDropped) return;
        if (_lootDatas.Length <= 0) return;

        Main_Controller main = _iInteractable.mainController;
        Vector2 dropPoint = Drop_SpawnPoint();

        if (dropPoint == (Vector2)transform.position) return;

        FoodDrop dropLootItem = _itemDropper.Drop_Food(dropPoint);

        _itemDropped = true;
        _droppedItem = dropLootItem;

        FoodData_Controller dropFoodIcon = dropLootItem.foodIcon;
        int maxDataCount = 0;

        for (int i = 0; i < _lootDatas.Length; i++)
        {
            FoodData lootData = _lootDatas[i];
            int lootAmount = _lootDatas[i].currentAmount;

            maxDataCount += lootAmount;
            dropFoodIcon.SetMax_SubDataCount(maxDataCount);

            for (int j = 0; j < lootAmount; j++)
            {
                dropFoodIcon.Set_CurrentData(new(lootData.foodScrObj));
            }
        }

        Update_CurrentSprite();
    }

    private void Drop_LootBoxItem(List<FoodData> dropData)
    {
        if (dropData.Count <= 0) return;

        FoodDrop dropLootItem = _itemDropper.Drop_Food(Drop_SpawnPoint());
        _droppedItem = dropLootItem;

        FoodData_Controller dropIcon = dropLootItem.foodIcon;
        dropIcon.Update_AllDatas(dropData);
    }
}
