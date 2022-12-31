using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Current_Buff_Icon_UI : MonoBehaviour
{
    public bool hasBuff;
    public Buff_ScrObj currentBuff;
    public Image buffIcon;
    public Button button;

    public void Empty_Icon()
    {
        hasBuff = false;
        currentBuff = null;
        buffIcon.color = Color.clear;
        button.enabled = false;
    }
    public void Assign_Icon(Buff_ScrObj buff)
    {
        hasBuff = true;
        currentBuff = buff;
        buffIcon.color = Color.white;
        buffIcon.sprite = currentBuff.sprite;
        button.enabled = true;
    }
}
