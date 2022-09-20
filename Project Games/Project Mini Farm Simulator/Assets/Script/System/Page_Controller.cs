using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Page_Controller : MonoBehaviour
{
    public GameObject[] pages;

    private int _currentPageNum;
    public int currentPageNum => _currentPageNum;

    private void Start()
    {
        Restart_Deactivate_AllPages();
        _currentPageNum = 1;
        Activate_Page();
    }

    // private functions
    private void Restart_Deactivate_AllPages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
    }

    private void Activate_Page()
    {
        int scanNum = 0;
        for (int i = 0; i < pages.Length; i++)
        {
            scanNum++;
            if (scanNum == currentPageNum)
            {
                pages[i].SetActive(true);
                break;
            }
        }
    }

    private void Check_PageNum_StartEnd()
    {
        // check start
        if (currentPageNum < 1)
        {
            _currentPageNum = pages.Length;
        }
        // check end
        else if (currentPageNum > pages.Length)
        {
            _currentPageNum = 1;
        }
    }

    // public functions
    public void NextPage()
    {
        _currentPageNum++;
        Restart_Deactivate_AllPages();
        Check_PageNum_StartEnd();
        Activate_Page();
    }

    public void BackPage()
    {
        _currentPageNum--;
        Restart_Deactivate_AllPages();
        Check_PageNum_StartEnd();
        Activate_Page();
    }
}
