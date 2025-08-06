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

    [Header("")] 
    [SerializeField] private AmountBar _refillBar;
    [SerializeField] private GameObject _dropReadyIcon;

    [Header("")] 
    [SerializeField][Range(0, 100)] private int _refillTikCount;
    [SerializeField] [Range(0, 10)] private float _refillTime;
    
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
        
        _dropReadyIcon.SetActive(!_data.dropped);

        // subscriptions
        Main_Controller.instance.worldMap.OnNewLocation += Reset_DroppedState;
        
        globaltime globaltime = globaltime.instance;
        globaltime.OnDayTime += _data.droppedMapHistory.Clear;
        globaltime.OnTimeTik += Refill;
        
        _iInteractable.OnInteract += Drop_LootBoxItem;
    }
    
    private void OnDestroy()
    {
        // subscriptions
        Main_Controller.instance.worldMap.OnNewLocation -= Reset_DroppedState;
        
        globaltime globaltime = globaltime.instance;
        globaltime.OnDayTime -= _data.droppedMapHistory.Clear;
        globaltime.OnTimeTik -= Refill;

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
        _data.Set_TikCount(0);
    }
    
    private void Refill()
    {
        if (_data.dropped == false) return;
        
        if (_data.tikCount < _refillTikCount)
        {
            _data.Update_TikCount(1);
            return;
        }

        StartCoroutine(Refill_Coroutine());
    }
    private IEnumerator Refill_Coroutine()
    {
        int refillAmount = 0;

        for (int i = 0; i < _passiveLootDatas.Length; i++)
        {
            refillAmount += _passiveLootDatas[i].currentAmount;
        }
        
        _refillBar.Set_MaxAmount(refillAmount);
        _refillBar.Toggle_BarColor(true);
        _refillBar.Toggle(true);

        float eachWaitTime = _refillTime / refillAmount;

        while (_refillBar.Is_MaxAmount() == false)
        {
            _refillBar.Update_Amount(1);
            _refillBar.Load();

            yield return new WaitForSeconds(eachWaitTime);
        }
        
        _refillBar.Toggle(false);
        
        _data.Set_TikCount(0);
        
        Reset_DroppedState();
        Update_CurrentSprite();
        
        _dropReadyIcon.SetActive(true);
        
        yield break;
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
        
        _dropReadyIcon.SetActive(false);
        
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
    }
}
