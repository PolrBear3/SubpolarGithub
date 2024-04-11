using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class StationBuilder : MonoBehaviour, IInteractable, ISaveLoadable
{
    private PlayerInput _input;

    private Main_Controller _main;
    private Detection_Controller _detection;
    private BasicAnimation_Controller _animController;

    [SerializeField] private Action_Bubble _bubble;
    [SerializeField] private Clock_Timer _timer;
    [SerializeField] private CoinLauncher _launcher;

    private int _hoverNum;
    private List<Station_ScrObj> _buildableStations = new();

    private Coroutine _buildCoroutine;
    private Station_ScrObj _currentBuildStation;

    [Header("Hovering Station")]
    [SerializeField] private GameObject _hoverControlKeys;
    [SerializeField] private SpriteRenderer _hoveringStationSR;
    [SerializeField] private Animator _hoveringStationAnim;

    [Header("ActionBubble Icon")]
    [SerializeField] private Sprite _menuIcon;
    [SerializeField] private Sprite _cancelIcon;
    [SerializeField] private Sprite _collectIcon;

    [Header("Price Indicator")]
    [SerializeField] private GameObject _priceIndicator;
    [SerializeField] private TextMeshPro _priceText;



    // UnityEngine
    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();

        _main = FindObjectOfType<Main_Controller>();
        _detection = gameObject.GetComponent<Detection_Controller>();
        _animController = gameObject.GetComponent<BasicAnimation_Controller>();

        Claim_Position();
    }

    private void Start()
    {
        if (ES3.KeyExists("StationShop_buildableStations") == false)
        {
            Sort_AvailableStations();
        }
        else
        {
            Load_Data();
        }

        Update_HoverStation();
        UnInteract();
    }



    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _input.enabled = true;

        if (_currentBuildStation != null || _timer.timeRunning) return;

        _hoveringStationAnim.Play("TransparencyBlinker_blink");
        _hoverControlKeys.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        UnInteract();
    }



    // InputSystem
    private void OnAction1()
    {
        if (_bubble.bubbleOn)
        {
            if (_currentBuildStation == null)
            {
                Build_HoverStation();
            }
            else if (_timer.timeRunning)
            {
                Cancel_BuildingStaion();
            }
            else
            {
                Collect_BuiltStation();
            }

            UnInteract();
            return;
        }

        if (_timer.timeRunning) return;

        _hoverNum--;
        Update_HoverStation();
    }

    private void OnAction2()
    {
        if (_bubble.bubbleOn) return;
        if (_timer.timeRunning) return;

        _hoverNum++;
        Update_HoverStation();
    }



    // IInteractable
    public void Interact()
    {
        if (_bubble.bubbleOn)
        {
            UnInteract();
            return;
        }

        ActionBubble_Update();
        _hoverControlKeys.SetActive(false);
    }

    public void UnInteract()
    {
        _bubble.Toggle(false);

        if (_currentBuildStation == null && _timer.timeRunning == false)
        {
            _hoverControlKeys.SetActive(true);
        }

        _priceIndicator.SetActive(false);

        if (_detection.player != null) return;

        _input.enabled = false;

        _hoveringStationAnim.Play("TransparencyBlinker_hide");
        _hoverControlKeys.SetActive(false);
    }



    // ISaveLoadable
    public void Save_Data()
    {
        if (_buildableStations.Count <= 0) return;

        List<int> stationIDs = new();

        for (int i = 0; i < _buildableStations.Count; i++)
        {
            stationIDs.Add(_buildableStations[i].id);
        }

        ES3.Save("StationBuilder/_buildableStations/stationIDs", stationIDs);
    }

    public void Load_Data()
    {
        List<int> stationIDs = new();

        stationIDs = ES3.Load("StationBuilder/_buildableStations/stationIDs", stationIDs);

        for (int i = 0; i < stationIDs.Count; i++)
        {
            _buildableStations.Add(_main.dataController.Station_ScrObj(stationIDs[i]));
        }
    }



    // Position Claim
    private void Claim_Position()
    {
        _main.Claim_Position(transform.position);
    }



    // Action Bubble Control
    private void ActionBubble_Update()
    {
        if (_currentBuildStation == null)
        {
            _bubble.transform.localPosition = new Vector2(0f, 1.1f);
            _bubble.Update_Bubble(_menuIcon, null);

            _priceIndicator.SetActive(true);
            _priceText.text = _buildableStations[_hoverNum].price.ToString();
        }
        else if (_timer.timeRunning)
        {
            _bubble.transform.localPosition = new Vector2(0f, 1.1f);
            _bubble.Update_Bubble(_cancelIcon, null);
        }
        else
        {
            _bubble.transform.localPosition = new Vector2(0f, 0.75f);
            _bubble.Update_Bubble(_collectIcon, null);
        }
    }



    // Hovering Station Control
    private bool Has_BuildableStation(Station_ScrObj station)
    {
        for (int i = 0; i < _buildableStations.Count; i++)
        {
            if (station != _buildableStations[i]) continue;
            return true;
        }

        return false;
    }

    private void Sort_AvailableStations()
    {
        List<Station_ScrObj> allStations = _main.dataController.stations;

        for (int i = 0; i < allStations.Count; i++)
        {
            if (allStations[i].unRetrievable) continue;
            if (Has_BuildableStation(allStations[i])) continue;

            _buildableStations.Add(allStations[i]);
        }

        // sort by price from lowest to highest
        _buildableStations.Sort((x, y) => x.price.CompareTo(y.price));
    }

    private void Update_HoverStation()
    {
        if (_hoverNum < 0) _hoverNum = _buildableStations.Count - 1;
        else if (_hoverNum > _buildableStations.Count - 1) _hoverNum = 0;

        Station_ScrObj hoveringStation = _buildableStations[_hoverNum];

        // station mini icon update center position
        _hoveringStationSR.transform.localPosition = hoveringStation.centerPosition * 0.01f;
        _hoveringStationSR.sprite = hoveringStation.miniSprite;

        // price indicator update
        _priceText.text = _buildableStations[_hoverNum].price.ToString();
    }



    // Build Time Control
    private void Build_HoverStation()
    {
        Station_ScrObj hoverStation = _buildableStations[_hoverNum];

        // player coin amount check
        if (Main_Controller.currentStationCoin < hoverStation.price)
        {
            // not enough station coins !!
            return;
        }

        // current coin calculation
        Main_Controller.currentStationCoin -= hoverStation.price;

        // player toss coin to builder
        Player_Controller player = _detection.player;
        Coin_ScrObj stationCoin = _main.dataController.coinTypes[1];
        player.coinLauncher.Parabola_CoinLaunch(stationCoin, transform.position - player.transform.position);

        // start timer
        _timer.Set_Time((int)hoverStation.buildTime);
        _timer.Toggle_Transparency(false);
        _timer.Run_Time();

        // start build
        _currentBuildStation = hoverStation;

        _animController.Play_Animation("StationBuilder_build");
        _hoveringStationAnim.Play("TransparencyBlinker_hide");

        _buildCoroutine = StartCoroutine(Build_HoverStation_Coroutine());
    }
    private IEnumerator Build_HoverStation_Coroutine()
    {
        while (_timer.timeRunning) yield return null;

        if (_bubble.bubbleOn == false) yield return null;

        _bubble.Toggle(false);
    }

    private void Cancel_BuildingStaion()
    {
        // refund station coin
        Main_Controller.currentStationCoin += _currentBuildStation.price;

        // refund toss coin to player
        Player_Controller player = _detection.player;
        Coin_ScrObj stationCoin = _main.dataController.coinTypes[1];
        _launcher.Parabola_CoinLaunch(stationCoin, player.transform.position - transform.position);

        // current build station cancel
        _currentBuildStation = null;
        StopCoroutine(_buildCoroutine);

        // stop timer
        _timer.Stop_Time();
        _timer.Toggle_Transparency(true);

        // play inactive animation
        _animController.Play_Animation("StationBuilder_inactive");
        _hoveringStationAnim.Play("TransparencyBlinker_blink");
    }

    private void Collect_BuiltStation()
    {
        // collect built station
        _main.currentVehicle.menu.stationMenu.Add_StationItem(_currentBuildStation, 1);

        // toss built station to player
        Player_Controller player = _detection.player;
        _launcher.Parabola_CoinLaunch(_currentBuildStation.miniSprite, player.transform.position - transform.position);

        // current build station default
        _currentBuildStation = null;

        // play inactive animation
        _animController.Play_Animation("StationBuilder_inactive");
        _hoveringStationAnim.Play("TransparencyBlinker_blink");
    }
}
