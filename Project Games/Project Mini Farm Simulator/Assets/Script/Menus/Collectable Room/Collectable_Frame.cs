using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Collectable_Frame_UI
{
    public Sprite defaultFrame, placeModeFrame;
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
    private Image image;
    
    public Collectable_Frame_UI ui;
    public Collectable_Frame_Data data;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void PlaceMode_On()
    {
        if (!data.collectablePlaced)
        {
            image.sprite = ui.placeModeFrame;
        }
    }
    public void PlaceMode_Off()
    {
        if (!data.collectablePlaced)
        {
            image.sprite = ui.defaultFrame;
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
        image.sprite = data.currentCollectable.sprite;

        // -1 selected collectable amount from all collectables
        for (int i = 0; i < menu.allCollectables.Length; i++)
        {
            if (data.currentCollectable == menu.allCollectables[i].collectable)
            {
                menu.allCollectables[i].currentAmount -= 1;
                menu.AllFrame_Amount_Text_Update();
                
                // if selected collectable amount is 0, placemode off
                if (menu.allCollectables[i].currentAmount <= 0)
                {
                    menu.AllFrame_PlaceMode_Off();
                    menu.Reset_Collectable_Selection();
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
                menu.AllFrame_Amount_Text_Update();
                menu.AllButton_Select_Available_Check();
                break;
            }
        }
        data.collectablePlaced = false;
        data.currentCollectable = null;
        image.sprite = ui.defaultFrame;
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
