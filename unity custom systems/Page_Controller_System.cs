using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page_Controller : MonoBehaviour
{
    public GameObject[] pages;
    private int currentPageNum = 0;

    private void Page_StartEnd_Check()
    {
        // back to page 1
        if (currentPageNum > pages.Length)
        {
            currentPageNum = 0;
        }
        // go to last page
        else if (currentPageNum < 0)
        {
            currentPageNum = pages.Length;
        }
    }

    private void Reset_All_Pages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
    }

    public void Next_Page()
    {
        currentPageNum++;
        Page_StartEnd_Check();
        Reset_All_Pages();
        pages[currentPageNum].SetActive(true);
    }

    public void Previous_Page()
    {
        currentPageNum--;
        Page_StartEnd_Check();
        Reset_All_Pages();
        pages[currentPageNum].SetActive(true);
    }
}