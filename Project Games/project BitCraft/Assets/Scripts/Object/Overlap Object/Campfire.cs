using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour, IInteractable
{
    private Prefab_Controller _controller;

    [SerializeField] private int _updateValue;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { _controller = controller; }
    }
    public void Interact()
    {
        Rest();
    }

    // Functions
    private void Rest()
    {
        Game_Controller controller = _controller.tilemapController.controller;

        controller.timeController.Update_Time(1);

        controller.tilemapController.playerController.prefabController.statController.Update_Current_Fatigue(_updateValue);
        controller.statPanel.Update_Fatigue_UIBar();
    }
}
