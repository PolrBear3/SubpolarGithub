using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoGuide_ControllerData
{
    [ES3Serializable] private List<Guide_ScrObj> _triggeredGuides = new();
    public List<Guide_ScrObj> triggeredGuides => _triggeredGuides;

    [ES3Serializable] private bool _guideActive;
    public bool guideActive => _guideActive;
    
    
    // New
    public VideoGuide_ControllerData(bool guideActive)
    {
        _guideActive = guideActive;
    }
    
    
    // Current Data
    public void Toggle_GuideActivation(bool toggle)
    {
        _guideActive = toggle;
    }
}
