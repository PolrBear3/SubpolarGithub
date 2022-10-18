using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectable_Button : MonoBehaviour
{
    public CollectableRoom_Menu menu;
    public Collectable_ScrObj collectable;
    public Image image;
    public Image frameImage;
    public Button button;
    public Image currentButtonImage;
    public Text amountText;
    public Sprite lockedImage;
    public Sprite[] buttonImages;
    private bool buttonPressed = false;

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
            if (collectable == menu.allCollectables[i].collectable)
            {
                amountText.text = menu.allCollectables[i].currentAmount.ToString();
            }
        }
    }
    public void Frame_Tier_Update()
    {
        var x = menu.allCollectableTierButtonFrames;
        for (int i = 0; i < x.Length; i++)
        {
            if (collectable.colorLevel == x[i].colorLevel)
            {
                frameImage.sprite = x[i].colorButtonFrameSprite;
                break;
            }
        }
    }
    public void Select_Available_Check()
    {
        for (int i = 0; i < menu.allCollectables.Length; i++)
        {
            if (collectable == menu.allCollectables[i].collectable)
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
        image.sprite = collectable.sprite;
    }
    public void Unlock_Check()
    {
        var x = menu.allCollectables;
        for (int i = 0; i < x.Length; i++)
        {
            if (collectable == x[i].collectable)
            {
                if (!x[i].unLocked)
                {
                    image.sprite = lockedImage;
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

    public void Select_Collectable()
    {
        buttonPressed = true;
        menu.AllFrame_PlaceMode_On();
        currentButtonImage.sprite = buttonImages[1];
        menu.data.selectedCollectable = collectable;
    }
    public void UnSelect_Collectable()
    {
        buttonPressed = false;
        menu.AllFrame_PlaceMode_Off();
        currentButtonImage.sprite = buttonImages[0];
        menu.data.selectedCollectable = null;
    }
    public void Select_This_Collectable()
    {
        if (!buttonPressed)
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
