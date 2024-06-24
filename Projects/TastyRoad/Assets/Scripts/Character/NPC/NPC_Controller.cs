using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Controller : MonoBehaviour
{
    private PlayerInput _input;

    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;


    [Header("")]
    [SerializeField] private Character_Data _characterData;
    public Character_Data characterData => _characterData;

    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    [SerializeField] private BasicAnimation_Controller _basicAnim;
    public BasicAnimation_Controller basicAnim => _basicAnim;

    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [SerializeField] private Action_Bubble _actionBubble;
    public Action_Bubble actionBubble => _actionBubble;


    [Header("")]
    [SerializeField] private CoinLauncher _itemLauncher;
    public CoinLauncher itemLauncher => _itemLauncher;

    [SerializeField] private Clock_Timer _timer;
    public Clock_Timer timer => _timer;


    [Header("")]
    [SerializeField] private NPC_Movement _movement;
    public NPC_Movement movement => _movement;

    [SerializeField] private NPC_Interaction _interaction;
    public NPC_Interaction interaction => _interaction;

    public delegate void Action_Event();
    public event Action_Event Action1;


    // UnityEngine
    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();

        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
        _mainController.Track_CurrentCharacter(gameObject);
    }

    private void Start()
    {
        InputToggle(false);
    }

    private void OnDestroy()
    {
        _mainController.UnTrack_CurrentCharacter(gameObject);
    }


    // InputSystem
    private void OnAction1()
    {
        Action1?.Invoke();
    }


    //
    public void InputToggle(bool toggleOn)
    {
        _input.enabled = toggleOn;
    }
}