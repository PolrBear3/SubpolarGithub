using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Controller : MonoBehaviour
{
    [SerializeField] private GameMenu_Controller _gameMenu;
    public GameMenu_Controller gameMenu => _gameMenu;
    
    [SerializeField] private RectTransform _menuPanelTab;
    public RectTransform menuPanelTab => _menuPanelTab;


    // MonoBehaviour
    public void Start()
    {
        
    }

    public void OnDestroy()
    {
        
    }

    public void OnEnable()
    {

    }

    public void OnDisable()
    {

    }
}
