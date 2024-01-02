using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class History_Buff_Icon : MonoBehaviour
{
    [SerializeField] private Buff_ScrObj currentBuff;
    [SerializeField] private Image buffImage;

    public void Clear_Icon()
    {
        currentBuff = null;
        buffImage.color = Color.clear;
    }
    public void Assign_Status(Buff_ScrObj buff)
    {
        currentBuff = buff;
        buffImage.sprite = currentBuff.sprite;
        buffImage.color = Color.white;
    }
}