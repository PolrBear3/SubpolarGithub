using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogSystem : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private RectTransform _spawnPoint;
    [SerializeField] private RectTransform[] _snapPoints;

    private List<DialogBox> _currentDialogs = new();

    [Header("")]
    [SerializeField] private LeanTweenType _tweenType;
    [Range(0, 1)][SerializeField] private float _transitionTime;


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


    //
    public DialogBox Add_DialogBox(DialogData data)
    {
        GameObject addDialog = Instantiate(_dialogBox, _spawnPoint);
        DialogBox dialogBox = addDialog.GetComponent<DialogBox>();

        _currentDialogs.Insert(0, dialogBox);
        dialogBox.Update_Box(data);

        ReOrder_CurrentDialogs();

        return dialogBox;
    }

    private void ReOrder_CurrentDialogs()
    {
        for (int i = 0; i < _currentDialogs.Count; i++)
        {
            if (i >= _snapPoints.Length)
            {
                Destroy(_currentDialogs[i].gameObject);
                _currentDialogs.RemoveAt(i);

                return;
            }

            _currentDialogs[i].transform.SetParent(_snapPoints[i]);
            LeanTween.moveLocal(_currentDialogs[i].gameObject, Vector2.zero, _transitionTime).setEase(_tweenType);
        }
    }
}
