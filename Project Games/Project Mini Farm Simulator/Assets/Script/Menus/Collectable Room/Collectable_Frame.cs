using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Collectable_Frame_UI
{
    public Sprite defaultFrame, placeableFrame, unPlaceableFrame;
}

[System.Serializable]
public class Collectable_Frame_Data
{
    public bool collectablePlaced = false;
}

public class Collectable_Frame : MonoBehaviour
{
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
            image.sprite = ui.placeableFrame;
        }
    }
    public void PlaceMode_Off()
    {
        image.sprite = ui.defaultFrame;
    }
}
