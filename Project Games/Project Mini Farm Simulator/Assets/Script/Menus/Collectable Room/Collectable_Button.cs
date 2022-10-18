using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Collectable_Button_UI
{
    public Image collectableImage, frameImage, selectButton;
    public Text amountText;
    public Sprite lockedImage;
    public Sprite[] buttonImages;
}

[System.Serializable]
public class Collectable_Button_Data
{
    public Collectable_ScrObj thisCollectable;
    public bool buttonPressed = false;
}

public class Collectable_Button : MonoBehaviour
{
    public CollectableRoom_Menu menu;
    public Button button;

    public Collectable_Button_UI ui;
    public Collectable_Button_Data data;

    private void Awake()
    {
        UI_Set();
    }

    public void Button_Shield(bool activate)
    {
        if (activate) { button.enabled = false; }
        else if (!activate) { button.enabled = true; }
    }

    public void Amount_Text_Update()
    {
        for (int i = 0; i < menu.allCollectables.Length; i++)
        {
            if (data.thisCollectable == menu.allCollectables[i].collectable)
            {
                ui.amountText.text = menu.allCollectables[i].currentAmount.ToString();
            }
        }
    }
    public void Frame_Tier_Update()
    {
        var x = menu.allCollectableTierButtonFrames;
        for (int i = 0; i < x.Length; i++)
        {
            if (data.thisCollectable.colorLevel == x[i].colorLevel)
            {
                ui.frameImage.sprite = x[i].colorButtonFrameSprite;
                break;
            }
        }
    }
    public void Select_Available_Check()
    {
        for (int i = 0; i < menu.allCollectables.Length; i++)
        {
            if (data.thisCollectable == menu.allCollectables[i].collectable)
            {
                if (menu.allCollectables[i].currentAmount <= 0)
                {
                    button.enabled = false;
                }
                else
                {
                    button.enabled = true;
                }
            }
        }
    }

    private void UI_Set()
    {
        ui.collectableImage.sprite = data.thisCollectable.sprite;
    }
    public void Unlock_Check()
    {
        var x = menu.allCollectables;
        for (int i = 0; i < x.Length; i++)
        {
            if (data.thisCollectable == x[i].collectable)
            {
                if (!x[i].unLocked)
                {
                    ui.collectableImage.sprite = ui.lockedImage;
                    button.enabled = false;
                }
                else
                {
                    UI_Set();
                    button.enabled = true;
                }
            }
        }
    }

    private void Select_Collectable()
    {
        data.buttonPressed = true;
        menu.AllFrame_PlaceMode_On();
        ui.selectButton.sprite = ui.buttonImages[1];
        menu.data.selectedCollectable = data.thisCollectable;
    }
    public void UnSelect_Collectable()
    {
        data.buttonPressed = false;
        menu.AllFrame_PlaceMode_Off();
        ui.selectButton.sprite = ui.buttonImages[0];
        menu.data.selectedCollectable = null;
    }
    public void Select_This_Collectable()
    {
        if (!data.buttonPressed)
        {
            menu.Reset_Collectable_Selection();
            Select_Collectable();
        }
        else
        {
            UnSelect_Collectable();
        }
    }
}
