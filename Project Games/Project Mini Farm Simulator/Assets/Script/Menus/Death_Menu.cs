using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Death_Menu_Data
{
    public LeanTweenType tweenType;
}

[System.Serializable]
public class Death_Menu_UI
{
    public RectTransform rectTransform;
    public Button[] allAvailableButtons;

    public Image previousSeedImage;
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
        LeanTween.move(ui.rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(data.tweenType);

        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        currentFarmTile.deathData.died = false;
        currentFarmTile.deathIcon.SetActive(false);

        Update_Death_Info();
    }
    public void Close()
    {
        controller.Reset_All_Tile_Highlights();
        Button_Shield(true);
        LeanTween.move(ui.rectTransform, new Vector2(0f, -125f), 0.75f).setEase(data.tweenType);
    }

    private void Update_Death_Info()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];

        // previous planted seed image
        ui.previousSeedImage.sprite = currentFarmTile.deathData.previousSeed.sprites[3];

        // health


        // current statuses
        statusPanel.Assign_All(currentFarmTile);
    }
}
