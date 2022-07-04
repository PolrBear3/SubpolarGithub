using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locked_Menu : MonoBehaviour
{
    public MainGame_Controller controller;

    RectTransform rectTransform;
    public LeanTweenType tweenType;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Open()
    {
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
    }
    public void Close()
    {
        controller.Reset_All_Tile_Highlights();
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }
}
