using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CraftNPC : MonoBehaviour
{
    [Header("")]
    [SerializeField] private CraftNPC_Controller _controller;
    public CraftNPC_Controller controller => _controller;


    [Header("")]
    [SerializeField] private AnimatorOverrideController _animOverride;

    [Header("")]
    [SerializeField] private UnityEvent OnSetInstance;


    // MonoBehaviour
    public void Start()
    {
        NPC_Movement movement = _controller.controller.movement;
        SpriteRenderer roamArea = _controller.controller.mainController.currentLocation.data.roamArea;

        movement.Free_Roam(roamArea, 1f);
    }


    //
    public void SetInstance_CurrentNPC()
    {
        NPC_Controller npc = _controller.controller;

        npc.interactable.Clear_ActionSubscriptions();
        OnSetInstance?.Invoke();

        npc.basicAnim.Set_OverrideController(_animOverride);
    }
}
