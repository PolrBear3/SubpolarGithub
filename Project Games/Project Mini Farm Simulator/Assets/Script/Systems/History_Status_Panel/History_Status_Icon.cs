using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class History_Status_Icon : MonoBehaviour
{
    [SerializeField] private Status currentStatus;
    [SerializeField] private Image statusImage;

    public void Clear_Icon()
    {
        currentStatus = null;
        statusImage.color = Color.clear;
    }
    public void Assign_Status(Status status)
    {
        currentStatus = status;
        statusImage.sprite = currentStatus.statusIcon;
        statusImage.color = Color.white;
    }
}
