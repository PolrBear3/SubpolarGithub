using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroPanel_Controller : MonoBehaviour
{
    [Header("Opening Curtain Control")]
    [SerializeField] private GameObject _openingCurtain;
    [SerializeField] private float _openingTime;

    [Header("Intro Panel Control")]
    [SerializeField] private List<GameObject> _pages = new();
    private int _currentPageNum;

    private void Start()
    {
        LoadGame_Animation();
    }

    public void Page_Control(int updateNum)
    {
        _pages[_currentPageNum].SetActive(false);

        _currentPageNum += updateNum;

        if (_currentPageNum > _pages.Count - 1)
        {
            _currentPageNum = 0;
        }
        else if (_currentPageNum < 0)
        {
            _currentPageNum = _pages.Count - 1;
        }

        _pages[_currentPageNum].SetActive(true);
    }

    private void LoadGame_Animation()
    {
        _openingCurtain.SetActive(true);
        LeanTween.moveLocalX(_openingCurtain, -2000f, _openingTime);
    }
}
