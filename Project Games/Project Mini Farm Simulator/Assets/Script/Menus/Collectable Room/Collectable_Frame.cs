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
    private bool collectablePlaced = false;
}

public class Collectable_Frame : MonoBehaviour
{
    public Collectable_Frame_UI ui;
    public Collectable_Frame_Data data;

    public void PlaceMode_On()
    {

    }
    public void PlaceMode_Off()
    {

    }
}
