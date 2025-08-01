using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class DialogSystem : MonoBehaviour
{
    [Space(20)]
    [SerializeField] RectTransform _dialogPanel;
    [SerializeField] private UI_EffectController _uiEffectController;
    
    [Space(20)]
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private RectTransform[] _snapPoints;

    [Space(20)]
    [SerializeField] private InformationBox _infoBox;
    public InformationBox infoBox => _infoBox;

    [Space(20)]
    [SerializeField] private GameObject _navigateBox;
    [SerializeField] private TextMeshProUGUI _navigateText;

    [Space(20)]
    [SerializeField] private List<DialogData> _customDialogs = new();

    [Space(20)]
    [SerializeField] private LeanTweenType _tweenType;
    [Range(0, 1)][SerializeField] private float _transitionTime;


    private List<DialogBox> _currentDialogs = new();
    private int _currentDialogNum;


    // UnityEngine
    private void Start()
    {
        _infoBox.Set_DefalutHeight();

        Refresh_CurrentDialogs();
        Update_NavigateBox();
        
        HoverToggle_CurrentDialog(false);

        // subscriptions
        Input_Controller input = Input_Controller.instance;
        
        input.OnNavigate += Navigate_InfoBox;
        input.OnActionMapUpdate += () => _navigateBox.SetActive(input.Current_ActionMapNum() == 0 && _infoBox.gameObject.activeSelf == false);

        PauseMenu_Controller pause = PauseMenu_Controller.instance;
        pause.OnPause += () => Toggle_InfoBox(false);
        pause.OnPause += () => _navigateBox.SetActive(input.Current_ActionMapNum() == 0 && _infoBox.gameObject.activeSelf == false);

        Localization_Controller localizationController = Localization_Controller.instance;
        
        localizationController.OnLanguageChanged += Refresh_CurrentDialogs;
        localizationController.OnLanguageChanged += Update_NavigateBox;
    }

    private void OnDestroy()
    {
        // subscriptions
        Input_Controller.instance.OnNavigate -= Navigate_InfoBox;
        
        Localization_Controller localizationController = Localization_Controller.instance;
        
        localizationController.OnLanguageChanged -= Refresh_CurrentDialogs;
        localizationController.OnLanguageChanged -= Update_NavigateBox;
    }


    // Basic Functions
    public DialogBox Add_DialogBox(DialogData data)
    {
        GameObject addDialog = Instantiate(_dialogBox, _snapPoints[0].transform);
        DialogBox dialogBox = addDialog.GetComponent<DialogBox>();

        dialogBox.Set_Data(data);
        dialogBox.Update_IconImage();

        _currentDialogs.Insert(0, dialogBox);
        ReOrder_CurrentDialogs();

        Toggle_InfoBox(_infoBox.gameObject.activeSelf);
        
        _uiEffectController.Update_Scale(_dialogPanel);
        Audio_Controller.instance.Play_OneShot(gameObject, 0);

        return dialogBox;
    }


    // Dialogs Control
    public void Refresh_CurrentDialogs()
    {
        foreach (DialogBox dialogBox in _currentDialogs)
        {
            Destroy(dialogBox.gameObject);
        }
        
        _currentDialogs.Clear();
        
        for (int i = 0; i < _snapPoints.Length; i++)
        {
            GameObject addDialog = Instantiate(_dialogBox, _snapPoints[0].transform);
            DialogBox dialogBox = addDialog.GetComponent<DialogBox>();

            dialogBox.Set_Data(_customDialogs[i]);
            dialogBox.Update_IconImage();

            _currentDialogs.Insert(0, dialogBox);
        }

        HoverToggle_CurrentDialog(_infoBox.gameObject.activeSelf);
        ReOrder_CurrentDialogs();

        _infoBox.Update_RectLayout();
    }


    // Information Box Control
    private void Toggle_InfoBox(bool toggle)
    {
        _infoBox.gameObject.SetActive(toggle);
        _navigateBox.SetActive(!toggle);

        if (toggle == false)
        {
            HoverToggle_CurrentDialog(false);
            Audio_Controller.instance.Play_OneShot(gameObject, 2);
           
            return;
        }

        RefreshCurrent_DialogInfo();
        HoverToggle_CurrentDialog(true);
        
        _uiEffectController.Update_Scale(_infoBox.rect);
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }
    public void Toggle_InfoBox(int index)
    {
        if (_infoBox.gameObject.activeSelf && _currentDialogNum == index)
        {
            Toggle_InfoBox(false);
            return;
        }

        int dialogIndex = Mathf.Clamp(index, 0, _currentDialogs.Count - 1);
        _currentDialogNum = dialogIndex;
        
        Toggle_InfoBox(true);
    }

    private void Navigate_InfoBox(Vector2 navigatedInput)
    {
        if (navigatedInput.y != 0)
        {
            _currentDialogNum = 0;

            Toggle_InfoBox(!_infoBox.gameObject.activeSelf);
            return;
        }

        if (!_infoBox.gameObject.activeSelf) return;

        int updateNum = (_currentDialogNum + (int)navigatedInput.x + _currentDialogs.Count) % _currentDialogs.Count;
        _currentDialogNum = updateNum;

        Toggle_InfoBox(true);
    }


    public void RefreshCurrent_DialogInfo()
    {
        _infoBox.infoText.text = _currentDialogs[_currentDialogNum].data.DialogInfo();
        _infoBox.Update_RectLayout();
    }
    
    
    // Navigate Box Control
    private void Update_NavigateBox()
    {
        InfoTemplate_Trigger template = gameObject.GetComponent<InfoTemplate_Trigger>();

        string toggleString = template.TemplateString(0);
        string navigateString = "    " + template.TemplateString(1);

        _navigateText.text = toggleString + navigateString;
    }
    

    // Current Dialogs Control
    private void ReOrder_CurrentDialogs()
    {
        for (int i = 0; i < _currentDialogs.Count; i++)
        {
            // destroy if old dialog box
            if (i >= _snapPoints.Length)
            {
                Destroy(_currentDialogs[i].gameObject);
                _currentDialogs.RemoveAt(i);

                return;
            }

            _currentDialogs[i].transform.SetParent(_snapPoints[i]);

            if (i == 0) continue;

            LeanTween.moveLocal(_currentDialogs[i].gameObject, Vector2.zero, _transitionTime).setEase(_tweenType);
        }
    }

    private void HoverToggle_CurrentDialog(bool toggleOn)
    {
        Main_Controller main = Main_Controller.instance;

        if (toggleOn == false)
        {
            foreach (var dialog in _currentDialogs)
            {
                Utility.Change_ImageAlpha(dialog.iconImage, 1f);
            }
            return;
        }

        for (int i = 0; i < _currentDialogs.Count; i++)
        {
            if (i != _currentDialogNum)
            {
                Utility.Change_ImageAlpha(_currentDialogs[i].iconImage, 0.5f);
                continue;
            }

            Utility.Change_ImageAlpha(_currentDialogs[i].iconImage, 1f);
        }
    }
}
