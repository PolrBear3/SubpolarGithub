using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private SpriteRenderer _fadeSR;
    
    [Space(20)] 
    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;
    
    [SerializeField] private IInteractable_Controller _interactable;
    public IInteractable_Controller interactable => _interactable;

    [Space(20)] 
    [SerializeField] private bool _restrictAutoFade;

    [Space(10)] 
    [SerializeField] [Range(0, 100)] private int _destroyTime;
    [SerializeField][Range(0, 100)] private int _fadeTime;
    
    [Space(10)] 
    [SerializeField] [Range(0, 1)] private float _fadeAlphaValue;
    
    
    // UnityEngine
    public void Start()
    {
        if (_restrictAutoFade) return;
        Fade_Destroy();
    }

    public void OnDestroy()
    {
        
    }


    // Main
    public void Fade_Destroy()
    {
        StartCoroutine(FadeDestroy_Coroutine());
    }
    private IEnumerator FadeDestroy_Coroutine()
    {
        int timeCount = _destroyTime;
        bool fadeActivated = false;
        
        Color originalColor = _fadeSR.color;

        while (timeCount > 0)
        {
            timeCount--;

            if (fadeActivated || timeCount > _fadeTime)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            fadeActivated = true;
            _fadeSR.color = new Color(originalColor.r, originalColor.g, originalColor.b, _fadeAlphaValue);
            
            yield return new WaitForSeconds(1f);
        }

        Destroy(gameObject);
        yield break;
    }
}
