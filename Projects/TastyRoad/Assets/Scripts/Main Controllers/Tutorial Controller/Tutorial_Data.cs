using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial_Data : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerInput _disableInput; 

    private Tutorial_Controller _controller;
    [SerializeField] private Tutorial_ScrObj _tutorial;

    [Header("")]
    [SerializeField] private bool _onEnableShow;



    // UnityEngine
    private void Awake()
    {
        _controller = FindObjectOfType<Tutorial_Controller>();
    }

    private void Start()
    {
        if (_controller.Tutorial_Shown(_tutorial) == false) return;
        Destroy(this);
    }



    // for UI Menu Tutorials
    private void OnEnable()
    {
        if (_controller.Tutorial_Shown(_tutorial))
        {
            Destroy(this);
            return;
        }

        if (_onEnableShow == false) return;

        _controller.Show_Tutorial(_disableInput, _tutorial);
        Destroy(this);
    }



    // IInteractable
    public void Interact()
    {
        if (_onEnableShow) return;

        _controller.Show_Tutorial(_disableInput, _tutorial);
        Destroy(this);
    }

    public void UnInteract()
    {
    }
}
