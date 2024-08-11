using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Controller : MonoBehaviour
{
    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;


    [Header("")]
    [SerializeField] private Character_Data _characterData;
    public Character_Data characterData => _characterData;

    [SerializeField] private BasicAnimation_Controller _basicAnim;
    public BasicAnimation_Controller basicAnim => _basicAnim;

    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [SerializeField] private ActionBubble_Interactable _interactable;
    public ActionBubble_Interactable interactable => _interactable;


    [Header("")]
    [SerializeField] private CoinLauncher _itemLauncher;
    public CoinLauncher itemLauncher => _itemLauncher;

    [SerializeField] private ItemDropper _itemDropper;
    public ItemDropper itemDropper => _itemDropper;

    [SerializeField] private Clock_Timer _timer;
    public Clock_Timer timer => _timer;


    [Header("")]
    [SerializeField] private NPC_Movement _movement;
    public NPC_Movement movement => _movement;

    [SerializeField] private NPC_Interaction _interaction;
    public NPC_Interaction interaction => _interaction;


    // UnityEngine
    private void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
        _mainController.Track_CurrentCharacter(gameObject);
    }

    private void OnDestroy()
    {
        _mainController.UnTrack_CurrentCharacter(gameObject);
    }
}