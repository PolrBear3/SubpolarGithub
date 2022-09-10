using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status_Icon_Indicator : MonoBehaviour
{
    public Status[] allStatus;
    public Status_Icon[] statusIcons;

    private void Start()
    {
        Empy_All_NonStatus_Icons();
    }

    public void Reset_All_Icons()
    {
        for (int i = 0; i < statusIcons.Length; i++)
        {
            statusIcons[i].Empty_Icon();
        }
    }
    private void Empy_All_NonStatus_Icons()
    {
        for (int i = 0; i < statusIcons.Length; i++)
        {
            if (!statusIcons[i].hasStatus)
            {
                statusIcons[i].Empty_Icon();
            }
        }
    }

    public void Assign_Status(StatusType statusType)
    {
        for (int i = 0; i < allStatus.Length; i++)
        {
            // scan status type
            if (statusType == allStatus[i].statusType)
            {
                for (int j = 0; j < statusIcons.Length; j++)
                {
                    // assigning scanned icon to empty slot
                    if (!statusIcons[i].hasStatus)
                    {
                        statusIcons[i].Assign_Icon(allStatus[i]);
                    }
                }
            }
        }
    }
    public void UnAssign_Status(StatusType statusType)
    {
        for (int i = 0; i < statusIcons.Length; i++)
        {
            if (statusIcons[i].hasStatus)
            {
                if (statusType == statusIcons[i].currentStatus.statusType)
                {
                    statusIcons[i].Empty_Icon();
                    break;
                }
            }
        }
    }
}
