using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status_Icon_Indicator : MonoBehaviour
{
    public FarmTile farmTile;

    public Status_Icon[] statusIcons;
    private Status[] tmpStatus = new Status[6];

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
        var allStatus = farmTile.controller.allStatus;
        for (int i = 0; i < statusIcons.Length; i++)
        {
            if (!statusIcons[i].hasStatus)
            {
                for (int j = 0; j < allStatus.Length; j++)
                {
                    if (statusType == allStatus[j].statusType)
                    {
                        statusIcons[i].Assign_Icon(allStatus[j]);
                        break;
                    }
                }
                break;
            }
        }
    }

    private void Re_Arrange_Icons()
    {
        for (int i = 0; i < tmpStatus.Length; i++)
        {
            if (statusIcons[i].hasStatus)
            {
                tmpStatus[i] = statusIcons[i].currentStatus;
            }
            else if (!statusIcons[i].hasStatus)
            {
                tmpStatus[i] = null;
            }
        }

        Reset_All_Icons();

        int arrayNum = 0;
        for (int i = 0; i < tmpStatus.Length; i++)
        {
            if (tmpStatus[i] != null)
            {
                statusIcons[arrayNum].Assign_Icon(tmpStatus[i]);
                arrayNum++;
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
                    Re_Arrange_Icons();
                    break;
                }
            }
        }
    }
}
