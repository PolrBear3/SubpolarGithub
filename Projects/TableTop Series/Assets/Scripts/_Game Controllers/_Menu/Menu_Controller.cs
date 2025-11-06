using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Controller : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private RectTransform _menuPanelTab;
    public RectTransform menuPanelTab => _menuPanelTab;


    // MonoBehaviour
    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }
}
