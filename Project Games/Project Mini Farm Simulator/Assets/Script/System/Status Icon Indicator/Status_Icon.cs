using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status_Icon : MonoBehaviour
{
    public Image currentStatusIcon;

    public bool hasStatus = false;
    public Status currentStatus;

    public void Empty_Icon()
    {
        hasStatus = false;
        currentStatus = null;
        currentStatusIcon.color = Color.clear;
    }

    public void Assign_Icon(Status status)
    {
        hasStatus = true;
        currentStatus = status;
        currentStatusIcon.color = Color.white;
        currentStatusIcon.sprite = currentStatus.statusIcon;
    }

    public void Load_Icon()
    {
        if (hasStatus)
        {
            currentStatusIcon.color = Color.white;
            currentStatusIcon.sprite = currentStatus.statusIcon;
        }
        else
        {
            currentStatusIcon.color = Color.clear;
        }
    }
}
