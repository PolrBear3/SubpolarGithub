using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Death_Menu_Data
{

}

[System.Serializable]
public class Death_Menu_UI
{
    public RectTransform rectTransform;
    public LeanTweenType tweenType;
    public Button[] allAvailableButtons;
}

public class Death_Menu : MonoBehaviour
{
    [SerializeField] private MainGame_Controller controller;
    [SerializeField] private History_Status_Panel statusPanel;

    [SerializeField] private Death_Menu_Data data;
    [SerializeField] private Death_Menu_UI ui;

    private void Start()
    {
        // set to position at the center
        ui.rectTransform.anchoredPosition = new Vector2(0f, -125f);
    }

    // button shields
    public void Button_Shield(bool activate)
    {
        for (int i = 0; i < ui.allAvailableButtons.Length; i++)
        {
            if (activate) { ui.allAvailableButtons[i].enabled = false; }
            else if (!activate) { ui.allAvailableButtons[i].enabled = true; }
        }
    }

    public void Open()
    {
        Button_Shield(false);
        LeanTween.move(ui.rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(ui.tweenType);

        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        currentFarmTile.data.died = false;
        // death icon OFF
    }
    public void Close()
    {
        controller.Reset_All_Tile_Highlights();
        Button_Shield(true);
        LeanTween.move(ui.rectTransform, new Vector2(0f, -125f), 0.75f).setEase(ui.tweenType);
    }
}
