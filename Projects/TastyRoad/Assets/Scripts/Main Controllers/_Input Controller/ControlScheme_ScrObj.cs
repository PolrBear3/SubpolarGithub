using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Scheme!")]
public class ControlScheme_ScrObj : ScriptableObject
{
    [Header("")]
    [SerializeField] private string _schemeName;
    public string schemeName => _schemeName;

    // font asset //

    [Header("")]
    [SerializeField] private ActionKey_Data[] _actionKeyDatas;
    public ActionKey_Data[] actionKeyDatas => _actionKeyDatas;
}
