using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Seed_Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnPlanted_Menu menu;
    public Seed_ScrObj seedInfo;

    public Image seedImage;
    public Text seedPriceText;

    bool onPress;
    float timer = 0;
    float onPressTime = 0.25f;

    private void Awake()
    {
        Set_Seed_Info();
    }
    private void Update()
    {
        Timer();
        Show_ToolTip();
    }

    private void Set_Seed_Info()
    {
        seedImage.sprite = seedInfo.sprites[3];
        seedPriceText.text = "$ " + seedInfo.seedBuyPrice.ToString();
    }

    public void Select_This_Seed()
    {
        menu.Select_Seed(seedInfo);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPress = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        onPress = false;
        menu.hide_Seed_ToolTip();
    }

    private void Timer()
    {
        if (onPress)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
        }
    }
    private void Show_ToolTip()
    {
        if (timer >= onPressTime)
        {
            menu.Show_Seed_ToolTip(seedInfo);
        }
    }
}
