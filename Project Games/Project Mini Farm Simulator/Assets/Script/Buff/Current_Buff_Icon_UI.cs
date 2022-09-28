using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Current_Buff_Icon_UI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Buff_Menu menu;
    
    public bool hasBuff;
    public Buff_ScrObj currentBuff;
    public Image buffIcon;

    bool onPress;
    float timer = 0;
    float onPressTime = 0.25f;

    private void Update()
    {
        Timer();
        Show_ToolTip();
    }

    public void Empty_Icon()
    {
        hasBuff = false;
        currentBuff = null;
        buffIcon.color = Color.clear;
    }
    public void Assign_Icon(Buff_ScrObj buff)
    {
        hasBuff = true;
        currentBuff = buff;
        buffIcon.color = Color.white;
        buffIcon.sprite = currentBuff.sprite;
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
            menu.Show_Buff_ToolTip(currentBuff);
        }
    }
}
