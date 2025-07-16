using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EffectController : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private LeanTweenType _tweenType;
    [SerializeField] private Vector2 _initialScale;
    [SerializeField][Range(0, 10)] private float _durationTime;


    public void Update_Scale(GameObject scaleObject)
    {
        if (scaleObject.gameObject.activeSelf == false) return;
        
        LeanTween.cancel(scaleObject);
        
        scaleObject.transform.localScale = _initialScale;
        LeanTween.scale(scaleObject, Vector2.one, _durationTime).setEase(_tweenType);
    }
    
    public void Update_Scale(RectTransform scaleUI)
    {
        if (scaleUI.gameObject.activeSelf == false) return;
        
        LeanTween.cancel(scaleUI.gameObject);
        
        scaleUI.localScale = _initialScale;
        LeanTween.scale(scaleUI, Vector2.one, _durationTime).setEase(_tweenType);
    }
}
