using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Buff_Menu_UI
{
    public Image buffPreview;
}

public class Buff_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    RectTransform rectTransform;
    public LeanTweenType tweenType;

    public Buff_Menu_UI ui;
    public Button[] allAvailableButtons;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Button_Shield(bool activate)
    {
        for (int i = 0; i < allAvailableButtons.Length; i++)
        {
            if (activate) { allAvailableButtons[i].enabled = false; }
            else if (!activate) { allAvailableButtons[i].enabled = true; }
        }
    }

    public void Open()
    {
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
        Button_Shield(false);
    }
    public void Close()
    {
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
        Button_Shield(true);

        controller.plantedMenu.Button_Shield(false);
    }

    /* 
    1. select buff (preview)
    2. add buff to farmtile list
    3. close this menu
    4. activate current buff after next day
    */
}
