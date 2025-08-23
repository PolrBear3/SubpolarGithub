using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoGuide_Trigger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    [Space(20)]
    [SerializeField] private Guide_ScrObj _triggerGuide;
    public Guide_ScrObj triggerGuide => _triggerGuide;
    
    
    private void Start()
    {
        Update_Indicator();

        VideoGuide_Controller videoGuide = VideoGuide_Controller.instance;
        
        videoGuide.OnGuideTrigger += Update_Indicator;
        videoGuide.OnGuide_ActivationTrigger += Update_Indicator;
    }

    private void OnDestroy()
    {
        VideoGuide_Controller videoGuide = VideoGuide_Controller.instance;
        
        videoGuide.OnGuideTrigger -= Update_Indicator;
        videoGuide.OnGuide_ActivationTrigger -= Update_Indicator;
    }
    

    // Control
    private void Update_Indicator()
    {
        VideoGuide_Controller videoGuide = VideoGuide_Controller.instance;

        if (videoGuide.data.guideActive && videoGuide.Guide_Triggered(_triggerGuide) == false)
        {
            _spriteRenderer.color = Color.white;
            return;
        }
        _spriteRenderer.color = Color.clear;
    }

    public void Trigger_CurrentGuide() 
    {
        if (_triggerGuide == null) return;
        VideoGuide_Controller.instance.Trigger_Guide(this);
        
        if (_triggerGuide.clipDatas.Length > 0) return;
        _spriteRenderer.color = Color.clear;
    }
}
