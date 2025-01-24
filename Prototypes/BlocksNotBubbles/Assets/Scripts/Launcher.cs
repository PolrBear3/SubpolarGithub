using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{
    [Header("")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Main_Controller _mainController;


    // InputSystem
    private void OnMovement()
    {

    }

    private void OnInteract()
    {
        Debug.Log("OnInteract");
    }
}
