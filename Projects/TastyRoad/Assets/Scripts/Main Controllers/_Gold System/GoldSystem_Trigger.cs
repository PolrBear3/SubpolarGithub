using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSystem_Trigger : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GoldSystem_TriggerData _data;

    private bool _hasData;


    // Data
    public void Set_Data(GoldSystem_TriggerData setData)
    {
        if (setData == null)
        {
            _hasData = false;
            return;
        }

        _hasData = true;
        _data = setData;
    }


    // Trigger
    public void Trigger_Data()
    {
        if (_hasData == false) return;

        GoldSystem.instance.Indicate_TriggerData(_data);
    }
}