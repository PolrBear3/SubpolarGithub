using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Collectable_Button_UI
{
    public Image collectableImage, frameImage, selectButton, goldModeButton;
    public Text amountText;
    public RectTransform amountTextPositon;
    public Sprite[] buttonImages;
    public GameObject lockIcon, newIcon, goldModeButtonGameObject;
}

[System.Serializable]
public class Collectable_Button_Data
{
    public Collectable_ScrObj thisCollectable;
    public bool buttonPressed = false;
    public bool goldMode = false;
}

public class Collectable_Button : MonoBehaviour
{
    public CollectableRoom_Menu menu;
    public Button button, frameButton;

    public Collectable_Button_UI ui;
    public Collectable_Button_Data data;

    private void Awake()
    {
        UI_Set();
    }

    public void Button_Shield(bool activate)
    {
        if (activate) { button.enabled = false; frameButton.enabled = false; }
        else if (!activate) { button.enabled = true; frameButton.enabled = true; }
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
        var x = menu.allCollectableTierData;
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
                    frameButton.enabled = false;
                }
                else
                {
                    button.enabled = true;
                    frameButton.enabled = true;
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
                // if it is locked
                if (!x[i].unLocked)
                {
                    ui.lockIcon.SetActive(true);
                    ui.amountText.enabled = false;
                    button.enabled = false;
                    frameButton.enabled = false;
                    ui.collectableImage.sprite = data.thisCollectable.lockedSprite;
                }
                // if it is unlocked
                else
                {
                    ui.lockIcon.SetActive(false);
                    ui.amountText.enabled = true;
                    button.enabled = true;
                    frameButton.enabled = true;
                    
                    if (!data.goldMode)
                    {
                        UI_Set();
                    }
                    else
                    {
                        ui.collectableImage.sprite = data.thisCollectable.goldSprite;
                    }
                }
            }
        }
    }
    public void NewIcon_Check()
    {
        var x = menu.allCollectables;
        for (int i = 0; i < x.Length; i++)
        {
            if (data.thisCollectable == x[i].collectable)
            {
                if (x[i].isNew && x[i].unLocked)
                {
                    ui.newIcon.SetActive(true);
                }
                else
                {
                    ui.newIcon.SetActive(false);
                }
                break;
            }
        }
    }

    private void Select_Collectable()
    {
        data.buttonPressed = true;
        menu.AllFrame_PlaceMode_On();
        ui.selectButton.sprite = ui.buttonImages[1];
        ui.amountTextPositon.anchoredPosition = new Vector2(0f, -2.65f);
        menu.data.selectedCollectable = data.thisCollectable;
        menu.data.selectedCollectableButton = this;

        var x = menu.allCollectables;
        for (int i = 0; i < x.Length; i++)
        {
            if (data.thisCollectable == x[i].collectable)
            {
                x[i].isNew = false;
                break;
            }
        }
        menu.AllButton_NewIcon_Check();
    }
    public void UnSelect_Collectable()
    {
        data.buttonPressed = false;
        menu.AllFrame_PlaceMode_Off();
        ui.selectButton.sprite = ui.buttonImages[0];
        ui.amountTextPositon.anchoredPosition = new Vector2(0f, 3.58f);
        menu.data.selectedCollectable = null;
        menu.data.selectedCollectableButton = null;
    }
    public void Select_This_Collectable()
    {
        if (!data.buttonPressed)
        {
            menu.AllButton_UnSelect();
            Select_Collectable();

        }
        else
        {
            UnSelect_Collectable();
        }
    }
    public void Play_Sound_onSelect(AudioClip clip)
    {
        menu.controller.soundController.Play_SFX(clip);
    }

    public void GoldMode_Check()
    {
        var x = menu.allCollectables;
        for (int i = 0; i < x.Length; i++)
        {
            if (data.thisCollectable == x[i].collectable)
            {
                if (x[i].goldModeAvailable)
                {
                    ui.goldModeButtonGameObject.SetActive(true);
                }
                else
                {
                    ui.goldModeButtonGameObject.SetActive(false);
                }
            }
        }
    }
    public void GoldMode_OnOff()
    {
        if (!data.goldMode)
        {
            data.goldMode = true;
            ui.collectableImage.sprite = data.thisCollectable.goldSprite;
        }
        else
        {
            data.goldMode = false;
            UI_Set();
        }
    }
}
