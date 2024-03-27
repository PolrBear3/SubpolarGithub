using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Data : MonoBehaviour, IInteractable
{
    private Tutorial_Controller _controller;
    [SerializeField] private Tutorial_ScrObj _tutorial;



    // UnityEngine
    private void Awake()
    {
        _controller = GameObject.FindGameObjectWithTag("Tutorial_Controller").GetComponent<Tutorial_Controller>();
    }

    private void Start()
    {
        if (_controller.Tutorial_Shown(_tutorial) == false) return;
        Destroy(this);
    }



    // IInteractable
    public void Interact()
    {
        _controller.Show_Tutorial(_tutorial);
        Destroy(this);
    }

    public void UnInteract()
    {
    }
}
