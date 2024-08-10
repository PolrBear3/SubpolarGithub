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

    private int _currentDialogNum;

    [Header("")]
    [SerializeField] private RectTransform _infoBox;
    [SerializeField] private TMP_Text _infoText;

    private float _defaultHeight;
    [SerializeField] private float _heightIncreaseValue;

    [Header("")]
    [SerializeField] private LeanTweenType _tweenType;
    [Range(0, 1)][SerializeField] private float _transitionTime;


    // UnityEngine
    private void Start()
    {
        _defaultHeight = _infoBox.anchoredPosition.y;
        _infoBox.gameObject.SetActive(false);
    }


    // InputSystem
    private void OnNumKey1()
    {
        InfoBox_Toggle(0);
    }
    private void OnNumKey2()
    {
        InfoBox_Toggle(1);
    }
    private void OnNumKey3()
    {
        InfoBox_Toggle(2);
    }
    private void OnNumKey4()
    {
        InfoBox_Toggle(3);
    }


    // Set
    public DialogBox Add_DialogBox(DialogData data)
    {
        GameObject addDialog = Instantiate(_dialogBox, _snapPoints[0].transform);
        DialogBox dialogBox = addDialog.GetComponent<DialogBox>();

        dialogBox.Set_Data(data);
        dialogBox.Update_IconImage();

        _currentDialogs.Insert(0, dialogBox);
        ReOrder_CurrentDialogs();

        Update_CurrentInfo(_currentDialogNum);
        HoverToggle_CurrentDialog(_infoBox.gameObject.activeSelf);

        return dialogBox;
    }


    // Info Box Control
    private void InfoBox_Toggle(int dialogNum)
    {
        if (_currentDialogs.Count <= dialogNum) return;

        if (dialogNum != _currentDialogNum)
        {
            _currentDialogNum = dialogNum;

            _infoBox.gameObject.SetActive(true);
            Update_CurrentInfo(dialogNum);

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
        Update_CurrentInfo(dialogNum);

        HoverToggle_CurrentDialog(true);
    }

    private void Update_CurrentInfo(int dialogNum)
    {
        _infoText.text = _currentDialogs[dialogNum].data.info.ToString();

        _infoText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_infoText.rectTransform);

        float lineCount = _infoText.textInfo.lineCount;
        float updateValue = _heightIncreaseValue * lineCount;
        float targetPosY = _defaultHeight + _heightIncreaseValue - updateValue;

        _infoBox.anchoredPosition = new Vector2(_infoBox.anchoredPosition.x, targetPosY);
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
