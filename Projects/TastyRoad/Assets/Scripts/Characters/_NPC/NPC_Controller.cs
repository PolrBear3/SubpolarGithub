using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Controller : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private BasicAnimation_Controller _basicAnim;
    public BasicAnimation_Controller basicAnim => _basicAnim;

    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [SerializeField] private ActionBubble_Interactable _interactable;
    public ActionBubble_Interactable interactable => _interactable;


    [Space(20)]
    [SerializeField] private CoinLauncher _itemLauncher;
    public CoinLauncher itemLauncher => _itemLauncher;

    [SerializeField] private ItemDropper _itemDropper;
    public ItemDropper itemDropper => _itemDropper;

    [SerializeField] private Clock_Timer _timer;
    public Clock_Timer timer => _timer;


    [Space(20)]
    [SerializeField] private NPC_Movement _movement;
    public NPC_Movement movement => _movement;

    [SerializeField] private NPC_Interaction _interaction;
    public NPC_Interaction interaction => _interaction;

    [SerializeField] private NPC_FoodInteraction _foodInteraction;
    public NPC_FoodInteraction foodInteraction => _foodInteraction;

    [SerializeField] private NPC_GiftSystem _giftSystem;
    public NPC_GiftSystem giftSystem => _giftSystem;


    // UnityEngine
    private void Awake()
    {
        Main_Controller.instance.Track_CurrentCharacter(gameObject);
    }

    private void OnDestroy()
    {
        Main_Controller.instance.UnTrack_CurrentCharacter(gameObject);
    }
}