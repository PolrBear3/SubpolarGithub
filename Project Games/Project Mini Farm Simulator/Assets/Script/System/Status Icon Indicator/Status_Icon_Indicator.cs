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

    public void Reset_All_Icons()
    {
        for (int i = 0; i < statusIcons.Length; i++)
        {
            statusIcons[i].Empty_Icon();
        }
    }
    public bool Find_Status(int StatusNum)
    {
        for (int i = 0; i < statusIcons.Length; i++)
        {
            // if the status icon is not empty
            if (statusIcons[i].currentStatus == null) break;
            // if it finds the wanting status
            if (statusIcons[i].currentStatus.statusID != StatusNum) continue;

            return true;
        }
        return false;
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
    
    public void Assign_Status(int statusID)
    {
        var allStatus = farmTile.controller.allStatus;
        for (int i = 0; i < statusIcons.Length; i++)
        {
            if (!statusIcons[i].hasStatus)
            {
                statusIcons[i].Assign_Icon(farmTile.controller.ID_Status_Search(statusID));
                break;
            }
        }
    }
    public void UnAssign_Status(int statusID)
    {
        for (int i = 0; i < statusIcons.Length; i++)
        {
            if (statusIcons[i].hasStatus)
            {
                if (statusID == statusIcons[i].currentStatus.statusID)
                {
                    statusIcons[i].Empty_Icon();
                    Re_Arrange_Icons();
                    break;
                }
            }
        }
    }
    public void UnAssign_Status_NonBreak(int statusID)
    {
        for (int i = 0; i < statusIcons.Length; i++)
        {
            if (statusIcons[i].hasStatus)
            {
                if (statusID == statusIcons[i].currentStatus.statusID)
                {
                    statusIcons[i].Empty_Icon();
                }
            }
        }

        Re_Arrange_Icons();
    }
}
