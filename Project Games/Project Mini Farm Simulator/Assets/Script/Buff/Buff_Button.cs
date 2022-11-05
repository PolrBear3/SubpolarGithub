using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff_Button : MonoBehaviour
{
    public Buff_Menu menu;
    public Buff_ScrObj buffInfo;

    public Image buffImage;
    public Text buffPriceText;

    private void Awake()
    {
        Set_Buff_Info();
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
}
