using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ActionKey_Data
{
    [SerializeField] private InputActionReference _actionRef;
    public InputActionReference actionRef => _actionRef;

    [SerializeField] private GameObject _actionKey;
    public GameObject actionKey => _actionKey;
}

public class ActionKey : MonoBehaviour
{
    private SpriteRenderer _sr;

    [SerializeField] private InputActionReference _keyReference;
    public InputActionReference keyReference => _keyReference;

    private InputActionReference _defaultReference;

    private InputActionReference _currentReference;
    public InputActionReference currentReference => _currentReference;

    private GameObject _currentKey;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();

        _defaultReference = _keyReference;
    }

    private void Start()
    {
        Set_CurrentKey();

        // subscriptions
        Input_Controller.instance.OnSchemeUpdate += Set_CurrentKey;
    }

    private void OnDestroy()
    {
        // subscriptions
        Input_Controller.instance.OnSchemeUpdate -= Reset_CurrentKey;
    }


    // Current Key Control
    public void Reset_CurrentKey()
    {
        _sr.color = Color.clear;

        if (_currentKey == null) return;

        GameObject currentKey = _currentKey;

        _currentReference = null;
        _currentKey = null;

        Destroy(currentKey);
    }


    public void Set_CurrentKey(InputActionReference actionRef)
    {
        if (actionRef == null) return;

        GameObject schemeKey = Input_Controller.instance.CurrentScheme_ActionKey(actionRef);
        if (schemeKey == null) return;

        _sr.color = Color.clear;

        GameObject setKey = Instantiate(schemeKey, transform.position, quaternion.identity);
        setKey.transform.SetParent(transform);

        _currentReference = actionRef;
        _currentKey = setKey;
    }

    public void Set_CurrentKey()
    {
        InputActionReference currentRef = _currentReference;

        Reset_CurrentKey();

        if (_defaultReference == null)
        {
            if (currentRef == null) return;

            Set_CurrentKey(currentRef);
            return;
        }

        Set_CurrentKey(_defaultReference);
    }
}
