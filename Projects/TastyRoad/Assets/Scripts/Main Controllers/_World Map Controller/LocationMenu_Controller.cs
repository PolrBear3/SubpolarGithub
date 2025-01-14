using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LocationMenu_Controller : MonoBehaviour
{
    private PlayerInput _input;

    [Header("")]
    [SerializeField] private Vehicle_Controller _vehicle;

    [Header("")]
    [SerializeField] private Image _menuPanel;


    // UnityEngine
    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();
    }

    private void Start()
    {
        Toggle_Menu(false);
    }


    // InputSystem
    private void OnExit()
    {
        Toggle_Menu(false);
    }


    //
    public void Toggle_Menu(bool toggle)
    {
        _vehicle.mainController.Player().Toggle_Controllers(!toggle);

        _menuPanel.gameObject.SetActive(toggle);
        _input.enabled = toggle;
    }
}
