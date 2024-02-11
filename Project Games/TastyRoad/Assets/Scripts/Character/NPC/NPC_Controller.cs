using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Controller : MonoBehaviour
{
    [HideInInspector] public Main_Controller mainController;

    private Character_Data _characterData;
    public Character_Data characterData => _characterData;

    [HideInInspector] public Detection_Controller detection;
    [HideInInspector] public BasicAnimation_Controller animationController;

    public FoodData_Controller foodIcon;
    public Action_Bubble actionBubble;

    [SerializeField] private Clock_Timer _timer;
    public Clock_Timer timer => _timer;

    public delegate void Action_Event();
    public event Action_Event Action1;

    [HideInInspector] public NPC_Movement movement;
    [HideInInspector] public NPC_Interaction interaction;

    // UnityEngine
    private void Awake()
    {
        mainController = FindObjectOfType<Main_Controller>();
        mainController.Track_CurrentCharacter(gameObject);

        if (gameObject.TryGetComponent(out Character_Data characterData)) { _characterData = characterData; }
        if (gameObject.TryGetComponent(out Detection_Controller detectionController)) { detection = detectionController; }
        if (gameObject.TryGetComponent(out BasicAnimation_Controller animationController)) { this.animationController = animationController; }

        if (gameObject.TryGetComponent(out NPC_Movement movement)) { this.movement = movement; }
        if (gameObject.TryGetComponent(out NPC_Interaction interaction)) { this.interaction = interaction; }
    }

    // InputSystem
    private void OnAction1()
    {
        Action1?.Invoke();
    }
}