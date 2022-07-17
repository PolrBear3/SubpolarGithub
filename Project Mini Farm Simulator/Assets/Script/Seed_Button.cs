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

    private void Awake()
    {
        Set_Seed_Info();
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
        // 1 sec hold timer function
        menu.Show_Seed_ToolTip(seedInfo);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        menu.hide_Seed_ToolTip();
    }
}
