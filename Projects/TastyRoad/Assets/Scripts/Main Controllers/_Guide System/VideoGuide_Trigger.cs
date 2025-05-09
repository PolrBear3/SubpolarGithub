using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoGuide_Trigger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    [Space(20)]
    [SerializeField] private Guide_ScrObj _triggerGuide;
    public Guide_ScrObj triggerGuide => _triggerGuide;
    
    
    // UnityEngine
    private void Start()
    {
        Update_Indicator();

        VideoGuide_Controller.instance.OnGuideTrigger += Update_Indicator;
    }

    private void OnDestroy()
    {
        VideoGuide_Controller.instance.OnGuideTrigger -= Update_Indicator;
    }
    

    // Control
    private void Update_Indicator()
    {
        if (VideoGuide_Controller.instance.Guide_Triggered(_triggerGuide) == false) return;
        
        _spriteRenderer.color = Color.clear;
    }

    public void Trigger_CurrentGuide()
    {
        VideoGuide_Controller.instance.Trigger_Guide(this);
    }
}
