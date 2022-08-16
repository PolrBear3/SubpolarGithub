using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Category_Controller : MonoBehaviour
{
    public List<Number_Box> numberBoxes = new List<Number_Box>();
    public GameObject[] numberBoxPages;
    public GameObject[] lastNextButton;
    public Text currentPageNumText;

    private int _currentPageNum;
    public int currentPageNum => _currentPageNum;

    private void Start()
    {
        Assign_Number_forBox();
        _currentPageNum = 1;
        LastNext_Button_Check();
        Reset_Page();
        Page_Check();
        CurrentPageNum_Text_Update();
    }

    private void Assign_Number_forBox()
    {
        int startAssignNum = 1;

        for (int i = 0; i < numberBoxes.Count; i++)
        {
            numberBoxes[i].boxNumber = startAssignNum;
            numberBoxes[i].Update_BoxNum_Text();
            startAssignNum++;
        }
    }

    private void Reset_Page()
    {
        for (int i = 0; i < numberBoxPages.Length; i++)
        {
            numberBoxPages[i].SetActive(false);
        }
    }

    private void Page_Check()
    {
        int scanPageNum = 0;
        for (int i = 0; i < numberBoxPages.Length; i++)
        {
            scanPageNum++;
            if(scanPageNum == currentPageNum)
            {
                numberBoxPages[i].SetActive(true);
                break;
            }
        }
    }

    private void LastNext_Button_Check()
    {
        // first page check
        if (_currentPageNum == 1)
        {
            lastNextButton[0].SetActive(false);
            lastNextButton[1].SetActive(true);
        }
        // last page check
        else if (_currentPageNum == numberBoxPages.Length)
        {
            lastNextButton[0].SetActive(true);
            lastNextButton[1].SetActive(false);
        }
        else
        {
            lastNextButton[0].SetActive(true);
            lastNextButton[1].SetActive(true);
        }
    }

    private void CurrentPageNum_Text_Update()
    {
        currentPageNumText.text = _currentPageNum + " / " + numberBoxPages.Length.ToString();
    }

    public void Next_Page()
    {
        _currentPageNum += 1;
        LastNext_Button_Check();
        Reset_Page();
        Page_Check();
        CurrentPageNum_Text_Update();
    }

    public void Last_Page()
    {
        _currentPageNum -= 1;
        LastNext_Button_Check();
        Reset_Page();
        Page_Check();
        CurrentPageNum_Text_Update();
    }
}
