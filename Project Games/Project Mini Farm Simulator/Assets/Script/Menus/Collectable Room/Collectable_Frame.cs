using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Collectable_Frame_UI
{
    public Sprite defaultFrame, placeModeFrame;
    public Image frameImage, collectableImage;
}

[System.Serializable]
public class Collectable_Frame_Data
{
    public bool collectablePlaced = false;
    public Collectable_ScrObj currentCollectable;
}

public class Collectable_Frame : MonoBehaviour
{
    public CollectableRoom_Menu menu;
    public Button button;
    
    public Collectable_Frame_UI ui;
    public Collectable_Frame_Data data;

    public void Button_Shield(bool activate)
    {
        if (activate) { button.enabled = false; }
        else if (!activate) { button.enabled = true; }
    }

    public void Load_FrameSprite()
    {
        if (data.collectablePlaced)
        {
            ui.collectableImage.sprite = data.currentCollectable.sprite;
        }
    }
    public void Frame_Tier_Update()
    {
        var x = menu.allCollectableTierData;
        for (int i = 0; i < x.Length; i++)
        {
            if (data.collectablePlaced)
            {
                if (data.currentCollectable.colorLevel == x[i].colorLevel)
                {
                    ui.frameImage.sprite = x[i].colorFrameFrameSprite;
                    break;
                }
            }
            else
            {
                ui.collectableImage.color = Color.clear;
                break;
            }
        }
    }

    public void PlaceMode_On()
    {
        if (!data.collectablePlaced)
        {
            ui.frameImage.sprite = ui.placeModeFrame;
        }
    }
    public void PlaceMode_Off()
    {
        if (!data.collectablePlaced)
        {
            ui.frameImage.sprite = ui.defaultFrame;
        }
    }

    private void Place_Selected_Collectable()
    {
        // replacing collectable
        if (data.collectablePlaced)
        {
            for (int i = 0; i < menu.allCollectables.Length; i++)
            {
                if (data.currentCollectable == menu.allCollectables[i].collectable)
                {
                    menu.allCollectables[i].currentAmount += 1;
                }
            }
        }

        // place selected collectable
        data.collectablePlaced = true;
        data.currentCollectable = menu.data.selectedCollectable;
        Frame_Tier_Update();

        // gold mode place check
        if (!menu.data.selectedCollectableButton.data.goldMode)
        {
            // normal
            ui.collectableImage.sprite = data.currentCollectable.sprite;
        }
        else
        {
            // gold
            ui.collectableImage.sprite = data.currentCollectable.goldSprite;
        }

        ui.collectableImage.color = Color.white;

        // -1 selected collectable amount from all collectables
        for (int i = 0; i < menu.allCollectables.Length; i++)
        {
            if (data.currentCollectable == menu.allCollectables[i].collectable)
            {
                menu.allCollectables[i].currentAmount -= 1;
                menu.AllButton_Amount_Text_Update();
                
                // if selected collectable amount is 0, placemode off
                if (menu.allCollectables[i].currentAmount <= 0)
                {
                    menu.AllFrame_PlaceMode_Off();
                    menu.AllButton_UnSelect();
                }
                // if selected collectable amount is still left, stay placemode on
                else
                {
                    menu.AllFrame_PlaceMode_On();
                }
                break;
            }
        }

        // collectable select button available check from amount remain
        menu.AllButton_Select_Available_Check();
    }
    private void Remove_Collectable()
    {
        for (int i = 0; i < menu.allCollectables.Length; i++)
        {
            if (data.currentCollectable == menu.allCollectables[i].collectable)
            {
                menu.allCollectables[i].currentAmount += 1;
                menu.AllButton_Amount_Text_Update();
                menu.AllButton_Select_Available_Check();
                break;
            }
        }
        data.collectablePlaced = false;
        data.currentCollectable = null;
        ui.frameImage.sprite = ui.defaultFrame;
        ui.collectableImage.color = Color.clear;
    }
    public void Press_Frame()
    {
        if (menu.data.placeMode)
        {
            Place_Selected_Collectable();
        }
        else
        {
            Remove_Collectable();
        }
    }
}
