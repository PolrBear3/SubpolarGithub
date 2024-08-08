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
    }


    // InputSystem
    /*
    private void OnNumKey1()
    {
        if (_currentDialogs.Count <= 0) return;

        _currentDialogs[0].InfoPanel_Toggle(true);
    }
    private void OnNumKey1Release()
    {
        if (_currentDialogs.Count <= 0) return;

        _currentDialogs[0].InfoPanel_Toggle(false);
    }

    private void OnNumKey2()
    {
        if (_currentDialogs.Count <= 1) return;

        _currentDialogs[1].InfoPanel_Toggle(true);
    }
    private void OnNumKey2Release()
    {
        if (_currentDialogs.Count <= 1) return;

        _currentDialogs[1].InfoPanel_Toggle(false);
    }

    private void OnNumKey3()
    {
        if (_currentDialogs.Count <= 2) return;

        _currentDialogs[2].InfoPanel_Toggle(true);
    }
    private void OnNumKey3Release()
    {
        if (_currentDialogs.Count <= 2) return;

        _currentDialogs[2].InfoPanel_Toggle(false);
    }
    */


    // Set
    public DialogBox Add_DialogBox(DialogData data)
    {
        GameObject addDialog = Instantiate(_dialogBox, _snapPoints[0].transform);
        DialogBox dialogBox = addDialog.GetComponent<DialogBox>();

        dialogBox.Set_Data(data);
        dialogBox.Update_IconImage();

        _currentDialogs.Insert(0, dialogBox);
        ReOrder_CurrentDialogs();

        Update_CurrentInfo(0);

        return dialogBox;
    }

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


    //
    private void Update_CurrentInfo(int dialogNum)
    {
        _infoText.text = _currentDialogs[dialogNum].data.info.ToString();

        _infoText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_infoText.rectTransform);

        float lineCount = _infoText.textInfo.lineCount;
        float updateValue = _heightIncreaseValue * lineCount;

        _infoBox.anchoredPosition = new Vector2(_infoBox.anchoredPosition.x, _defaultHeight + _heightIncreaseValue - updateValue);
    }
}
