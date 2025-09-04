using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using TMPro;

[System.Serializable]
public class GuideMenu_GroupData
{
    [SerializeField] private string _groupName;
    public string groupName => _groupName;
    
    [SerializeField] private LocalizedString _localizedGroupName;

    [Space(10)]
    [SerializeField] private Sprite _groupIcon;
    public Sprite groupIcon => _groupIcon;

    [SerializeField] private Sprite _selectIcon;
    public Sprite selectIcon => _selectIcon;

    [Space(10)]
    [SerializeField] private Guide_ScrObj[] _guides;
    public Guide_ScrObj[] guides => _guides;
    
    
    // Gets
    public string LocalizedGroupName()
    {
        if (_localizedGroupName == null) return _groupName;
        if (string.IsNullOrEmpty(_localizedGroupName.TableReference) && string.IsNullOrEmpty(_localizedGroupName.TableEntryReference)) return _groupName;
        
        return _localizedGroupName.GetLocalizedString();
    }
    
    public bool Has_Guide(Guide_ScrObj searchGuide)
    {
        for (int i = 0; i < _guides.Length; i++)
        {
            if (searchGuide != _guides[i]) continue;
            return true;
        }
        return false;
    }
    
    public int Guide_Index(Guide_ScrObj searchGuide)
    {
        int indexCount = 0;
        
        for (int i = 0; i < _guides.Length; i++)
        {
            if (searchGuide == _guides[i]) return indexCount;
            indexCount++;
        }
        return indexCount;
    }
}

public class GuideMenu_Controller : Menu_Controller
{
    [Space(60)] 
    [SerializeField] private Image _groupIcon;
    [SerializeField] private TextMeshProUGUI _groupNameText;
    [SerializeField] private GuideMenu_GroupData[] _groupDatas;

    [Space(20)] 
    [SerializeField] private GameObject[] _currentGuidesPointers;

    
    private int _currentGroupIndex;
    private int _currentPageIndex;
    
    private List<Guide_ScrObj> _currentGuides = new();
    
    
    // MonoBehaviour
    private new void Start()
    {
        base.Start();
        
        // subscriptions
        OnAction += Select_CurrentGuide;
        OnNavigateX += Update_CurrentGroup;
        OnWrapAroundY += Update_CurrentGuides;
        
        VideoGuide_Controller.instance.OnGuideToggle += Toggle_Menu;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();    
        
        // subscriptions
        OnAction -= Select_CurrentGuide;
        OnNavigateX -= Update_CurrentGroup;
        OnWrapAroundY -= Update_CurrentGuides;

        VideoGuide_Controller.instance.OnGuideToggle -= Toggle_Menu;
    }
    
    
    // Main
    public new void Toggle_Menu(bool toggle)
    {
        base.Toggle_Menu(toggle);
        
        if (toggle == false) return;

        Update_GroupNavigatePanel();
        Update_CurrentGuides();
        Update_Pointers();
    }
    private void Toggle_Menu()
    {
        if (PauseMenu_Controller.instance.isPaused == false) return;
        
        VideoGuide_Controller videoGuide = VideoGuide_Controller.instance;
        bool guideToggled = videoGuide.guideToggled;
        
        videoGuide.guideToggleBox.SetActive(!guideToggled);
        Toggle_Menu(!guideToggled);
    }
    
    private void Select_CurrentGuide()
    {
        int guideIndex = (eventButtons.Length - 1) - currentIndex;
        
        if (guideIndex > _currentGuides.Count - 1) return;
        VideoGuide_Controller.instance.Toggle_VideoPanel(_currentGuides[guideIndex]);
        
        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }
    
    
    // Updates
    private void Update_CurrentGroup(int directionValue)
    {
        _currentPageIndex = 0;
        
        int maxIndex = _groupDatas.Length;
        _currentGroupIndex = (_currentGroupIndex + directionValue + maxIndex) % maxIndex;
        
        Update_GroupNavigatePanel();
        Update_CurrentGuides();
        Update_Pointers();
    }
    
    private void Update_GroupNavigatePanel()
    {
        GuideMenu_GroupData group = _groupDatas[_currentGroupIndex];
        
        _groupIcon.sprite = group.groupIcon;
        _groupNameText.text = group.LocalizedGroupName();
        
        Update_TextSize(_groupNameText);
    }
    
    
    private List<Guide_ScrObj> CurrentGroup_Guides(int pageIndex)
    {
        Guide_ScrObj[] allGuides = _groupDatas[_currentGroupIndex].guides;
        List<Guide_ScrObj> guides = new();
        
        int guidesPerPage = eventButtons.Length;
        
        int guideCount = 0;
        int pageCount = 0;

        for (int i = 0; i < allGuides.Length; i++)
        {
            if (guides.Count >= guidesPerPage) break;
            
            if (pageIndex == pageCount)
            {
                guides.Add(allGuides[i]);
                continue;
            }
            
            if (guideCount < guidesPerPage - 1)
            {
                guideCount++;
                continue;
            }
            
            guideCount = 0;
            pageCount++;
        }
        
        return guides;
    }
    
    private void Update_CurrentGuides()
    {
        _currentGuides.Clear();

        List<Guide_ScrObj> updateGuides = CurrentGroup_Guides(_currentPageIndex);
        List<Menu_EventButton> buttons = EventButtons_InOrder();

        for (int i = 0; i < buttons.Count; i++)
        {
            TextMeshProUGUI buttonText = buttons[i].buttonText;
            
            if (i > updateGuides.Count - 1)
            {
                buttons[i].Set_SelectIcon(null);
                buttonText.text = string.Empty;
                
                continue;
            }
            
            _currentGuides.Add(updateGuides[i]);

            Sprite guideIconSprite = updateGuides[i].iconSprite;
            Sprite selectIconSprite = guideIconSprite != null ? guideIconSprite : _groupDatas[_currentGroupIndex].selectIcon;
            
            buttons[i].Set_SelectIcon(selectIconSprite);
            buttonText.text = updateGuides[i].LocalizedName();
        }

        Update_TextSize();
    }
    private void Update_CurrentGuides(int directionValue)
    {
        Guide_ScrObj[] allGuides = _groupDatas[_currentGroupIndex].guides;
        int maxPage = Mathf.CeilToInt((float)allGuides.Length / eventButtons.Length);

        _currentPageIndex = (_currentPageIndex + directionValue + maxPage) % maxPage;
        Update_CurrentGuides();
    }
    
    
    private void Update_Pointers()
    {
        bool toggle = _groupDatas[_currentGroupIndex].guides.Length > eventButtons.Length;
        
        foreach (GameObject pointer in _currentGuidesPointers)
        {
            pointer.SetActive(toggle);
        }
    }
}
