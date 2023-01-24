using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status_ToolTip : MonoBehaviour
{
    [SerializeField] Image statusIcon;
    [SerializeField] Text statusName;
    [SerializeField] Text statusDescription;

    public void Show(Status statusToShow)
    {
        // turn on panel
        gameObject.SetActive(true);

        // update status
        Update_Status_Info_UI(statusToShow);
    }
    public void Hide()
    {
        // turn off panel
        gameObject.SetActive(false);
    }

    private void Update_Status_Info_UI(Status status)
    {
        statusIcon.sprite = status.statusIcon;
        statusName.text = status.statusName;
        statusDescription.text = status.statusDescription;
    }
}
