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
    [SerializeField][Range(0, 100)] private float _fadeTime;
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
        float elapsed = 0f;
        Color originalColor = _fadeSR.color;

        while (elapsed < _fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _fadeTime;

            float alpha = Mathf.Lerp(1f, _fadeAlphaValue, t);
            _fadeSR.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }
        Destroy(gameObject);
    }
}
