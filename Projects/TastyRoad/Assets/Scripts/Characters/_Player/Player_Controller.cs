using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour, ISaveLoadable
{
    private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;


    [Space(20)]
    [SerializeField] private Player_Movement _movement;
    public Player_Movement movement => _movement;

    [SerializeField] private Player_Interaction _interaction;
    public Player_Interaction interaction => _interaction;

    
    [Space(20)]
    [SerializeField] private AbilityManager _abilityManager;
    public AbilityManager abilityManager => _abilityManager;
    
    [SerializeField] private Buddy_Controller _buddyController;
    public Buddy_Controller buddyController => _buddyController;


    [Space(20)]
    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    [SerializeField] private BasicAnimation_Controller _animationController;
    public BasicAnimation_Controller animationController => _animationController;


    [Space(20)]
    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [SerializeField] private CoinLauncher _coinLauncher;
    public CoinLauncher coinLauncher => _coinLauncher;

    [SerializeField] private Clock_Timer _timer;
    public Clock_Timer timer => _timer;


    private bool _hidden;
    public bool hidden => _hidden;

    public Action OnHideToggle;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();

        Main_Controller.instance.Track_CurrentCharacter(gameObject);
    }

    private void Start()
    {
        _foodIcon.Toggle_SubDataBar(true);
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("Player_Controller/_foodIcon.AllDatas()", _foodIcon.AllDatas());
    }

    public void Load_Data()
    {
        if (ES3.KeyExists("Player_Controller/_foodIcon.AllDatas()") == false) return;
        
        _foodIcon.Update_AllDatas(ES3.Load("Player_Controller/_foodIcon.AllDatas()", _foodIcon.AllDatas()));
        _foodIcon.Show_Icon();
        _foodIcon.Show_Condition();
    }


    // Player Input
    public void SchemeChange_Update(PlayerInput playerInput)
    {
        Debug.Log(playerInput.currentControlScheme);
    }


    // Functions
    public void Toggle_Hide(bool toggleOn)
    {
        _hidden = toggleOn;
        OnHideToggle?.Invoke();

        if (toggleOn)
        {
            _sr.color = Color.clear;

            _foodIcon.Hide_Icon();
            _foodIcon.Hide_Condition();
            _foodIcon.Toggle_SubDataBar(false);

            return;
        }

        _sr.color = Color.white;

        _foodIcon.Show_Icon();
        _foodIcon.Show_Condition();
        _foodIcon.Toggle_SubDataBar(true);
    }

    /// <summary>
    /// Disables all movement controls, detections and interactions
    /// </summary>
    public void Toggle_Controllers(bool toggle)
    {
        _detection.Toggle_BoxCollider(toggle);

        _movement.enabled = toggle;
        _interaction.enabled = toggle;

        if (toggle) return;
        _movement.Force_MovementRestriction();
    }
}