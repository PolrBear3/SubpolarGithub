using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class DialogSystem : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private RectTransform[] _snapPoints;

    private List<DialogBox> _currentDialogs = new();

    private bool _newDialogOpened;
    private int _currentDialogNum;

    [Header("")]
    [SerializeField] private List<DialogData> _customDialogs = new();

    [Header("")]
    [SerializeField] private GameObject _actionKey;

    [Header("")]
    [SerializeField] private InformationBox _infoBox;

    [Header("")]
    [SerializeField] private LeanTweenType _tweenType;
    [Range(0, 1)][SerializeField] private float _transitionTime;


    // UnityEngine
    private void Start()
    {
        _infoBox.Set_DefalutHeight();

        Refresh_CustomDialogs();
        _actionKey.SetActive(false);

        _newDialogOpened = true;
    }


    // InputSystem
    private void OnNumKey1()
    {
        InfoBox_Toggle(0);

        _newDialogOpened = true;

        if (_infoBox.gameObject.activeSelf == false) return;
        _actionKey.SetActive(false);
    }
    private void OnNumKey2()
    {
        InfoBox_Toggle(1);

        if (_infoBox.gameObject.activeSelf == true)
        {
            _actionKey.SetActive(false);
            return;
        }

        if (_newDialogOpened == true) return;
        _actionKey.SetActive(true);
    }
    private void OnNumKey3()
    {
        InfoBox_Toggle(2);

        if (_infoBox.gameObject.activeSelf == true)
        {
            _actionKey.SetActive(false);
            return;
        }

        if (_newDialogOpened == true) return;
        _actionKey.SetActive(true);
    }
    private void OnNumKey4()
    {
        InfoBox_Toggle(3);

        if (_infoBox.gameObject.activeSelf == true)
        {
            _actionKey.SetActive(false);
            return;
        }

        if (_newDialogOpened == true) return;
        _actionKey.SetActive(true);
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

        HoverToggle_CurrentDialog(_infoBox.gameObject.activeSelf);
        _actionKey.SetActive(!_infoBox.gameObject.activeSelf);

        _infoBox.Update_InfoText(_currentDialogs[_currentDialogNum].data.info);
        _infoBox.Update_RectLayout();

        _newDialogOpened = false;

        return dialogBox;
    }


    // Custom Dialogs Control
    public void Refresh_CustomDialogs()
    {
        for (int i = 0; i < _snapPoints.Length; i++)
        {
            Add_DialogBox(_customDialogs[i]);
        }
    }


    // Information Box Control
    private void InfoBox_Toggle(int dialogNum)
    {
        if (_currentDialogs.Count <= dialogNum) return;

        if (dialogNum != _currentDialogNum)
        {
            _currentDialogNum = dialogNum;

            _infoBox.gameObject.SetActive(true);
            _infoBox.Update_InfoText(_currentDialogs[_currentDialogNum].data.info);
            _infoBox.Update_RectLayout();

            HoverToggle_CurrentDialog(true);

            return;
        }

        if (_infoBox.gameObject.activeSelf == true)
        {
            _infoBox.gameObject.SetActive(false);

            HoverToggle_CurrentDialog(false);

            return;
        }

        _infoBox.gameObject.SetActive(true);
        _infoBox.Update_InfoText(_currentDialogs[_currentDialogNum].data.info);
        _infoBox.Update_RectLayout();

        HoverToggle_CurrentDialog(true);
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
        if (toggleOn == false)
        {
            foreach (var dialog in _currentDialogs)
            {
                Main_Controller.Change_ImageAlpha(dialog.iconImage, 1f);
            }
            return;
        }

        for (int i = 0; i < _currentDialogs.Count; i++)
        {
            if (i != _currentDialogNum)
            {
                Main_Controller.Change_ImageAlpha(_currentDialogs[i].iconImage, 0.5f);
                continue;
            }
            Main_Controller.Change_ImageAlpha(_currentDialogs[i].iconImage, 1f);
        }
    }
}
