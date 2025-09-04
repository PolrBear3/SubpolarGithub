using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Controller : MonoBehaviour
{
    public static Main_Controller instance;

    
    [Space(20)]
    [SerializeField] private SaveLoad_Controller _saveLoadController;
    public SaveLoad_Controller saveLoadController => _saveLoadController;
    
    [SerializeField] private InvokeExecution_Controller _invokeExecutionController;
    public InvokeExecution_Controller invokeExecutionController => _invokeExecutionController;
    
    [SerializeField] private Audio_Controller _audioController;
    public Audio_Controller audioController => _audioController;
    
    [SerializeField] private Localization_Controller _localizationController;
    public Localization_Controller localizationController => _localizationController;
    

    // MonoBehaviour
    private void Awake()
    {
        instance = this;
    }
}
