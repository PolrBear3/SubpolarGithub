using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectable_Button : MonoBehaviour
{
    public CollectableRoom_Menu menu;
    public Collectable_ScrObj collectable;
    public Image image;
    public Button button;
    public Image currentButtonImage;
    public Text amountText;
    public Sprite lockedImage;
    public Sprite[] buttonImages;

    private void Awake()
    {
        UI_Set();
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
        menu.AllFrame_PlaceMode_On();
        menu.Reset_Collectable_Selection();
        currentButtonImage.sprite = buttonImages[1];
        menu.data.selectedCollectable = collectable;
    }
    public void UnSelect_Collectable()
    {
        currentButtonImage.sprite = buttonImages[0];
    }
}
