using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogSystem : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private RectTransform[] _snapPoints;

    private List<DialogBox> _currentDialogs = new();


    // InputSystem
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


    //
    public void Add_DialogBox(DialogData data)
    {
        GameObject addDialog = Instantiate(_dialogBox, transform);
        DialogBox dialogBox = addDialog.GetComponent<DialogBox>();

        _currentDialogs.Insert(0, dialogBox);
        dialogBox.Update_Box(data);

        ReOrder_CurrentDialogs();
    }

    private void ReOrder_CurrentDialogs()
    {
        for (int i = 0; i < _currentDialogs.Count; i++)
        {
            if (i >= _snapPoints.Length)
            {
                Destroy(_currentDialogs[i].gameObject);
                _currentDialogs.RemoveAt(i);

                break;
            }

            _currentDialogs[i].transform.SetParent(_snapPoints[i]);
            _currentDialogs[i].transform.localPosition = Vector2.zero;
        }
    }
}
