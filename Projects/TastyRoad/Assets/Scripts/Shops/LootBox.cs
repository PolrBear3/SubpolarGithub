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


    private HashSet<WorldMap_Data> _droppedMapHistory = new();


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
        _iInteractable.OnInteract += Drop_LootBoxItem;
    }

    private void OnDestroy()
    {
        // subscriptions
        _iInteractable.OnInteract -= Drop_LootBoxItem;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("LootBox/_droppedMapHistory", _droppedMapHistory);
    }

    public void Load_Data()
    {
        _droppedMapHistory = ES3.Load("LootBox/_droppedMapHistory", _droppedMapHistory);
    }


    // Indication
    private void Update_CurrentSprite()
    {
        if (!CurrentLocation_Dropped())
        {
            _sr.sprite = _toggleSprites[0];
            return;
        }

        _sr.sprite = _toggleSprites[1];
    }


    // Dropped Map History Datas
    private bool CurrentLocation_Dropped()
    {
        WorldMap_Data currentLocationData = Main_Controller.instance.worldMap.currentData;

        return _droppedMapHistory.Contains(currentLocationData);
    }


    // Drop
    private Vector2 Drop_Position()
    {
        Location_Controller currentLocation = Main_Controller.instance.currentLocation;

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
        if (CurrentLocation_Dropped()) return;
        if (_lootDatas.Length <= 0) return;

        Vector2 dropPoint = Drop_Position();

        if (dropPoint == (Vector2)transform.position) return;

        FoodDrop dropLootItem = _itemDropper.Drop_Food(dropPoint);
        FoodData_Controller dropFoodIcon = dropLootItem.foodIcon;

        WorldMap_Data locationData = Main_Controller.instance.worldMap.currentData;
        _droppedMapHistory.Add(locationData);

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
}
