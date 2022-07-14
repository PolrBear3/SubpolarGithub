using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff_Button : MonoBehaviour
{
    public UnPlanted_Menu menu;
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
}
