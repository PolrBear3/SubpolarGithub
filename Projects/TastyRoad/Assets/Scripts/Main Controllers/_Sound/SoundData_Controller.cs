using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundData_Controller : MonoBehaviour
{
    [SerializeField] private SoundData[] _soundDatas;
    public SoundData[] soundDatas => _soundDatas;


    // MonoBehaviour
    private void OnDestroy()
    {
        Audio_Controller.instance.Clear_EventInstances(_soundDatas);
    }
}