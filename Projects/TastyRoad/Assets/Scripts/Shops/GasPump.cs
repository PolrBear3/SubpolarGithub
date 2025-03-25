using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPump : MonoBehaviour
{
    [Header("")]
    [SerializeField] private IInteractable_Controller _interactable;
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private DialogTrigger _dialog;

    [Header("")]
    [SerializeField] private Clock_Timer _fillClock;
    [SerializeField] private AmountBar _fillBar;

    [Header("")]
    [SerializeField] private Station_ScrObj _oilDrum;
    [SerializeField] private GameObject _spawnPoint;

    [Header("")]
    [SerializeField][Range(0, 1000)] private int _price;
    [SerializeField][Range(0, 100)] private int _fillTime;

    private Coroutine _coroutine;


    // UnityEngine
    private void Start()
    {
        Toggle_SpawnPoint();

        // Subscriptions
        _detection.EnterEvent += Toggle_SpawnPoint;
        _detection.ExitEvent += Toggle_SpawnPoint;

        _interactable.OnInteract += Toggle_Price;

        _interactable.OnHoldInteract += Purchase;
        _interactable.OnInteract += Spawn_OilDrum;

        GlobalTime_Controller.instance.OnDayTime += Fill_OilDrum;
    }

    private void OnDestroy()
    {
        Reset_OnFill();

        // Subscriptions
        _detection.EnterEvent -= Toggle_SpawnPoint;
        _detection.ExitEvent -= Toggle_SpawnPoint;

        _interactable.OnInteract -= Toggle_Price;

        _interactable.OnHoldInteract -= Purchase;
        _interactable.OnInteract -= Spawn_OilDrum;

        GlobalTime_Controller.instance.OnDayTime -= Fill_OilDrum;
    }


    // Indication Updates
    private void Toggle_Price()
    {
        if (_coroutine != null) return;
        if (_fillBar.Is_MaxAmount()) return;

        GoldSystem.instance.Indicate_TriggerData(new(_oilDrum.dialogIcon, -_price));
    }

    private void Toggle_SpawnPoint()
    {
        _spawnPoint.SetActive(_detection.player != null);
    }


    // Functions
    private void Purchase()
    {
        if (_coroutine != null)
        {
            _dialog.Update_Dialog(0);
            return;
        }

        if (_fillBar.Is_MaxAmount()) return;

        if (GoldSystem.instance.Update_CurrentAmount(-_price) == false) return;

        Fill_OilDrum();

        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }


    private void Spawn_OilDrum()
    {
        if (_coroutine != null) return;
        if (_fillBar.Is_MaxAmount() == false) return;

        Main_Controller main = Main_Controller.instance;

        if (main.Position_Claimed(_spawnPoint.transform.position)) return;

        Station_Controller stationController = main.Spawn_Station(_oilDrum, _spawnPoint.transform.position);
        stationController.movement.Load_Position();

        _fillBar.Toggle_BarColor(false);
        _fillBar.Set_Amount(0);
        _fillBar.Load();

        Audio_Controller.instance.Play_OneShot(gameObject, 0);
    }

    private void Fill_OilDrum()
    {
        if (_coroutine != null) return;
        if (_fillBar.Is_MaxAmount()) return;

        _coroutine = StartCoroutine(Fill_OilDrum_Coroutine());

        gameObject.GetComponent<DialogTrigger>().Update_Dialog(0);
    }
    private IEnumerator Fill_OilDrum_Coroutine()
    {
        _fillBar.Toggle_BarColor(true);
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

        _coroutine = null;
        yield break;
    }


    private void Reset_OnFill()
    {
        Main_Controller main = Main_Controller.instance;

        if (_coroutine == null) return;
        if (main.Position_Claimed(_spawnPoint.transform.position) == false) return;

        StopCoroutine(_coroutine);
        _coroutine = null;

        main.UnClaim_Position(_spawnPoint.transform.position);
    }
}
