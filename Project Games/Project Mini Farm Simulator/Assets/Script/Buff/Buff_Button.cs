using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Buff_Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnPlanted_Menu menu;
    public Buff_ScrObj buffInfo;

    public Image buffImage;
    public Text buffPriceText;

    bool onPress;
    float timer = 0;
    float onPressTime = 0.25f;

    private void Awake()
    {
        Set_Buff_Info();
    }
    private void Update()
    {
        Timer();
        Show_ToolTip();
    }

    private void Set_Buff_Info()
    {
        buffImage.sprite = buffInfo.sprite;
        buffPriceText.text = "$ " + buffInfo.buffPrice.ToString();
    }

    public void Select_This_Buff()
    {
        menu.Select_Buff(buffInfo);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPress = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        onPress = false;
        menu.Hide_Buff_ToolTip();
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
            menu.Show_Buff_ToolTip(buffInfo);
        }
    }
}
