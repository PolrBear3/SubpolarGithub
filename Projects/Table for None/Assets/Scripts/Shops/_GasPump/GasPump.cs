using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPump : MonoBehaviour, ISaveLoadable
{
    [Space(20)]
    [SerializeField] private ActionBubble_Interactable _interactable;

    [Space(20)]
    [SerializeField] private Clock_Timer _fillClock;
    [SerializeField] private AmountBar _fillBar;

    [SerializeField] private GameObject _spawnReadyIcon;

    [Space(20)]
    [SerializeField] private Station_ScrObj _oilDrum;

    [Space(20)]
    [SerializeField][Range(0, 1000)] private int _defaultPrice;
    [SerializeField][Range(0, 100)] private int _fillTime;


    private PurchaseData _data;
    
    private Coroutine _coroutine;


    // UnityEngine
    private void Awake()
    {
        Load_Data();
    }
    
    private void Start()
    {
        Load_PurchaseState();

        // subscriptions
        _interactable.OnInteract += Toggle_Price;
        _interactable.OnInteract += Update_BubbleData;
        
        _interactable.OnInteract += Spawn_OilDrum;

        _interactable.OnHoldInteract += Purchase;
        _interactable.OnAction1 += Purchase;

        GlobalTime_Controller.instance.OnDayTime += Fill_OilDrum;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.OnInteract -= Toggle_Price;
        _interactable.OnInteract -= Update_BubbleData;
        
        _interactable.OnInteract -= Spawn_OilDrum;

        _interactable.OnHoldInteract -= Purchase;
        _interactable.OnAction1 -= Purchase;

        GlobalTime_Controller.instance.OnDayTime -= Fill_OilDrum;
    }

    
    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("GasPump/PurchaseData", _data);
    }

    public void Load_Data()
    {
        _data = ES3.Load("GasPump/PurchaseData", new PurchaseData(_defaultPrice));
    }
    

    // Indications
    private void Toggle_Price()
    {
        if (_coroutine != null) return;
        if (_fillBar.Is_MaxAmount()) return;

        GoldSystem.instance.Indicate_TriggerData(new(_oilDrum.dialogIcon, -_data.price));
    }

    private void Update_BubbleData()
    {
        Action_Bubble bubble = _interactable.bubble;
        
        if (_coroutine != null || _data.purchased)
        {
            bubble.Empty_Bubble();
            bubble.Set_IndicatorToggleDatas(null);
            
            return;
        }

        ActionBubble_Data data = bubble.bubbleDatas[0];
        
        bubble.Set_Bubble(data.iconSprite, null);
        bubble.Set_IndicatorToggleDatas(new() { data });
    }
    
    private void Load_PurchaseState()
    {
        _spawnReadyIcon.SetActive(_data.purchased);

        if (_data.purchased == false) return;
        _fillBar.Set_Amount(_fillBar.maxAmount);
    }


    // Functions
    private void Purchase()
    {
        if (_coroutine != null) return;
        if (_data.purchased) return;
        if (GoldSystem.instance.Update_CurrentAmount(-_data.price) == false) return;

        _data.Toggle_PurchaseState(true);
        Fill_OilDrum();

        _interactable.UnInteract();
        
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }


    private List<Vector2> OilDrum_SpawnPositions()
    {
        Main_Controller main = Main_Controller.instance;
        MainController_Data mainData = main.data;
        Location_Controller currentLocation = main.currentLocation;
        
        Vector2 vehiclePos = main.currentVehicle.transform.position;
        List<Vector2> spawnPositions = Utility.Surrounding_SnapPositions(transform.position);
        
        spawnPositions.Sort((a, b) =>
            Vector2.Distance(vehiclePos, a).CompareTo(Vector2.Distance(vehiclePos, b))
        );

        for (int i = spawnPositions.Count - 1; i >= 0; i--)
        {
            if (mainData.Position_Claimed(spawnPositions[i]))
            {
                spawnPositions.RemoveAt(i);
                continue;
            }
            
            if (currentLocation.Is_OuterSpawnPoint(spawnPositions[i]))
            {
                spawnPositions.RemoveAt(i);
                continue;
            }
        }
        
        return spawnPositions;
    }
    
    private void Spawn_OilDrum()
    {
        if (_coroutine != null) return;
        if (_fillBar.Is_MaxAmount() == false) return;

        Main_Controller main = Main_Controller.instance;
        List<Vector2> spawnPositions = OilDrum_SpawnPositions();

        // add to station menu on no spawn positions available
        if (spawnPositions.Count == 0)
        {
            StationMenu_Controller stationMenu = main.currentVehicle.menu.stationMenu;
            if (stationMenu.Add_StationItem(_oilDrum, 1) == null) return;
        }
        // spawn on position
        else
        {
            Station_Controller stationController = main.Spawn_Station(_oilDrum, spawnPositions[0]);
            stationController.movement.Load_Position();
        }

        _data.Toggle_PurchaseState(false);
        
        _fillBar.Toggle_BarColor(false);
        _fillBar.Set_Amount(0);
        _fillBar.Load();

        _spawnReadyIcon.SetActive(false);
        
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
    }

    
    private void Fill_OilDrum()
    {
        if (_coroutine != null) return;
        if (_fillBar.Is_MaxAmount()) return;

        _coroutine = StartCoroutine(Fill_OilDrum_Coroutine());
    }
    private IEnumerator Fill_OilDrum_Coroutine()
    {
        _fillBar.Toggle_BarColor(true);
        _fillBar.Set_MaxAmount(_fillTime);
        _fillBar.Set_Amount(0);
        _fillBar.Toggle(true);

        _fillClock.Toggle_RunAnimation(true);

        for (int i = 0; i < _fillTime; i++)
        {
            _fillBar.Load();

            yield return new WaitForSeconds(1);

            _fillBar.Update_Amount(1);
        }

        _fillClock.Toggle_RunAnimation(false);
        _fillBar.Toggle(false);

        _spawnReadyIcon.SetActive(true);

        _coroutine = null;
        yield break;
    }
}
