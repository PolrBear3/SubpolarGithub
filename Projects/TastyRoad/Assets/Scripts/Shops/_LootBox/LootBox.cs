using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour, ISaveLoadable
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private IInteractable_Controller _iInteractable;
    [SerializeField] private ItemDropper _itemDropper;
    [SerializeField] private Custom_PositionClaimer _positionClaimer;

    [Header("")]
    [SerializeField] private Sprite[] _toggleSprites;

    [Header("")]
    [SerializeField] private FoodData[] _lootDatas;
    [SerializeField] private FoodData[] _passiveLootDatas;


    private LootBox_Data _data;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();

        Load_Data();
    }

    private void Start()
    {
        Update_CurrentSprite();

        // subscriptions
        WorldMap_Controller.OnNewLocation += Reset_DroppedState;

        _iInteractable.OnInteract += Drop_LootBoxItem;
    }

    private void OnDestroy()
    {
        // subscriptions
        WorldMap_Controller.OnNewLocation -= Reset_DroppedState;

        _iInteractable.OnInteract -= Drop_LootBoxItem;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("LootBox/LootBox_Data", _data);
    }

    public void Load_Data()
    {
        _data = ES3.Load("LootBox/LootBox_Data", new LootBox_Data(false));
    }


    // Indication
    private void Update_CurrentSprite()
    {
        if (!_data.dropped)
        {
            _sr.sprite = _toggleSprites[0];
            return;
        }

        _sr.sprite = _toggleSprites[1];
    }


    // Drop Loot Data Control
    private bool CurrentLocation_Dropped()
    {
        WorldMap_Data currentLocationData = Main_Controller.instance.worldMap.currentData;

        return _data.droppedMapHistory.Contains(currentLocationData);
    }

    private List<FoodData> Current_LootDatas()
    {
        List<FoodData> currentDatas = new();

        foreach (FoodData passiveData in _passiveLootDatas)
        {
            currentDatas.Add(passiveData);
        }
        if (CurrentLocation_Dropped()) return currentDatas;

        foreach (FoodData lootData in _lootDatas)
        {
            currentDatas.Add(lootData);
        }
        return currentDatas;
    }


    private void Reset_DroppedState()
    {
        _data.Toggle_DropStatus(false);
    }


    // Drop
    private Vector2 Drop_Position()
    {
        List<Vector2> surroundingPositions = _positionClaimer.All_InteractPositions();
        Transform playerPos = _detection.player.transform;

        surroundingPositions.Sort((a, b) =>
        Vector2.Distance(playerPos.position, a)
        .CompareTo(Vector2.Distance(playerPos.position, b)));

        Location_Controller currentLocation = Main_Controller.instance.currentLocation;

        for (int i = 0; i < surroundingPositions.Count; i++)
        {
            if (Main_Controller.instance.Position_Claimed(surroundingPositions[i])) continue;
            if (currentLocation.Restricted_Position(surroundingPositions[i])) continue;

            return surroundingPositions[i];
        }

        return transform.position;
    }

    private void Drop_LootBoxItem()
    {
        if (_data.dropped) return;

        List<FoodData> dropDatas = Current_LootDatas();
        if (dropDatas.Count <= 0) return;

        Vector2 dropPoint = Drop_Position();
        if (dropPoint == (Vector2)transform.position) return;

        FoodDrop dropLootItem = _itemDropper.Drop_Food(dropPoint);
        FoodData_Controller dropFoodIcon = dropLootItem.foodIcon;

        WorldMap_Data locationData = Main_Controller.instance.worldMap.currentData;
        _data.Add_MapHistory(locationData);

        int maxDataCount = 0;

        for (int i = 0; i < dropDatas.Count; i++)
        {
            FoodData lootData = dropDatas[i];
            int lootAmount = dropDatas[i].currentAmount;

            maxDataCount += lootAmount;
            dropFoodIcon.SetMax_SubDataCount(maxDataCount);

            for (int j = 0; j < lootAmount; j++)
            {
                dropFoodIcon.Set_CurrentData(new(lootData.foodScrObj));
            }
        }

        _data.Toggle_DropStatus(true);
        Update_CurrentSprite();
    }
}
