using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPump : MonoBehaviour, ISaveLoadable
{
    [Space(20)]
    [SerializeField] private IInteractable_Controller _interactable;
    [SerializeField] private Detection_Controller _detection;

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

        _interactable.OnInteract += Toggle_Price;

        _interactable.OnHoldInteract += Purchase;
        _interactable.OnInteract += Spawn_OilDrum;

        globaltime.instance.OnDayTime += Fill_OilDrum;
    }

    private void OnDestroy()
    {
        // Subscriptions
        _interactable.OnInteract -= Toggle_Price;

        _interactable.OnHoldInteract -= Purchase;
        _interactable.OnInteract -= Spawn_OilDrum;

        globaltime.instance.OnDayTime -= Fill_OilDrum;
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
    

    // Toggles
    private void Toggle_Price()
    {
        if (_coroutine != null) return;
        if (_fillBar.Is_MaxAmount()) return;

        GoldSystem.instance.Indicate_TriggerData(new(_oilDrum.dialogIcon, -_data.price));
    }


    // Functions
    private void Load_PurchaseState()
    {
        _spawnReadyIcon.SetActive(_data.purchased);

        if (_data.purchased == false) return;
        _fillBar.Set_Amount(_fillBar.maxAmount);
    }
    
    private void Purchase()
    {
        if (_coroutine != null) return;
        if (_data.purchased) return;
        if (GoldSystem.instance.Update_CurrentAmount(-_data.price) == false) return;

        _data.Toggle_PurchaseState(true);
        Fill_OilDrum();

        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }


    private void Spawn_OilDrum()
    {
        if (_coroutine != null) return;
        if (_fillBar.Is_MaxAmount() == false) return;

        Main_Controller main = Main_Controller.instance;
        List<Vector2> spawnPositions = Utility.Surrounding_SnapPositions(transform.position);

        for (int i = spawnPositions.Count - 1; i >= 0; i--)
        {
            if (main.Position_Claimed(spawnPositions[i]))
            {
                spawnPositions.RemoveAt(i);
                continue;
            }

            if (main.currentLocation.Restricted_Position(spawnPositions[i]))
            {
                spawnPositions.RemoveAt(i);
                continue;
            }

            break;
        }
        
        if (spawnPositions.Count == 0) return;

        Station_Controller stationController = main.Spawn_Station(_oilDrum, spawnPositions[^1]);
        stationController.movement.Load_Position();

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
