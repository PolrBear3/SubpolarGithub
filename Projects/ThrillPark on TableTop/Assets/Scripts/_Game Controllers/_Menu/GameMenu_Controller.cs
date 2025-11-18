using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameMenu_Controller : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Menu_Controller[] _menuControllers;

    [Space(10)]
    [SerializeField] private float _tabDefaultHeight;
    [SerializeField][Range(0, 10)] private float _tabUpdateDuration;

    private float _tabUpdateHeight;


    // MonoBehaviour
    private void Start()
    {
        LoadTab_UpdateHeight();

        if (_menuControllers.Length == 0) return;
        Load_MenuController(_menuControllers[0]);
    }


    // IPointer_EventSystem
    public void Select_Tab(RectTransform selectTab)
    {
        Vector2 selectSize = new(selectTab.rect.width, _tabDefaultHeight + _tabUpdateHeight);
        LeanTween.size(selectTab, selectSize, _tabUpdateDuration);

        selectTab.SetAsLastSibling();

        for (int i = 0; i < _menuControllers.Length; i++)
        {
            RectTransform tab = _menuControllers[i].menuPanelTab;

            if (selectTab == tab) continue;

            Vector2 defaultSize = new(tab.rect.width, _tabDefaultHeight);
            LeanTween.size(tab, defaultSize, _tabUpdateDuration);
        }
    }

    public void Load_MenuController(Menu_Controller loadMenu)
    {
        if (loadMenu == null)
        {
            Debug.Log("loadMenu field empty!");
            return;
        }

        for (int i = 0; i < _menuControllers.Length; i++)
        {
            Menu_Controller menu = _menuControllers[i];
            if (menu == null || menu == loadMenu) continue;

            GameObject menuPanel = menu.gameObject;
            if (menuPanel.activeSelf == false) continue;

            menuPanel.SetActive(false);
        }

        GameObject loadMenuPanel = loadMenu.gameObject;

        if (loadMenuPanel.activeSelf) return;
        loadMenu.gameObject.SetActive(true);
    }


    // Menu Control
    private void LoadTab_UpdateHeight()
    {
        for (int i = 0; i < _menuControllers.Length; i++)
        {
            float tabHeight = _menuControllers[i].menuPanelTab.rect.height;

            if (tabHeight == _tabDefaultHeight) continue;
            _tabUpdateHeight = Mathf.Abs(tabHeight - _tabDefaultHeight);

            return;
        }
    }

    public void Update_CurrentMenu()
    {
        for (int i = 0; i < _menuControllers.Length; i++)
        {
            if (_menuControllers[i].gameObject.activeSelf == false) continue;
            
            _menuControllers[i].OnMenuUpdate?.Invoke();
            return;
        }
    }
}
