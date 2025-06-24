using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Animation : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;
    
    [SerializeField] private Animator _animator;
    public Animator animator => _animator;
    
    [Space(20)]
    [SerializeField] private Spike _controller;

    [Space(20)] 
    [SerializeField] private AnimatorOverrideController[] _spikeAnimOverrides;

    [Space(20)] 
    [SerializeField] [Range(0, 1)] private float _maxOutLineWidth;


    private Coroutine _effectCoroutine;
    
    
    // MonoBehaviour
    private void Start()
    {
        Update_AnimationOverride();
        
        // subscriptions
        _controller.OnDamage += Toggle_DamageEffect;
        _controller.OnDeath += Cancel_DamageEffect;
    }

    private void OnDestroy()
    {
        // subscriptions
        _controller.OnDamage -= Toggle_DamageEffect;
        _controller.OnDeath -= Cancel_DamageEffect;
    }

    private void Update()
    {
        Animation_Update();
    }
    
    
    // Animation
    private void Animation_Update()
    {
        if (_controller.movement.isMoving == false)
        {
            _animator.speed = 0;
            return;
        }
        _animator.speed = 1;
    }

    public void Update_AnimationOverride()
    {
        if (_controller.data.hasTail)
        {
            _animator.runtimeAnimatorController = _spikeAnimOverrides[0];
            return;
        }
        _animator.runtimeAnimatorController = _spikeAnimOverrides[1];
    }


    private void Cancel_DamageEffect()
    {
        if (_effectCoroutine == null) return;
        
        StopCoroutine(_effectCoroutine);
        _effectCoroutine = null;
        
        _sr.material.SetFloat("_OutlineAlpha", 0f);
    }
    
    public void Toggle_DamageEffect()
    {
        _effectCoroutine = StartCoroutine(DamageEffect_Coroutine());
    }
    private IEnumerator DamageEffect_Coroutine()
    {
        Material damageMaterial = _sr.material;

        damageMaterial.SetFloat("_OutlineAlpha", 1f);
        damageMaterial.SetFloat("_OutlineWidth", _maxOutLineWidth);
        
        float duration = _controller.healCoolTime;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentWidth = Mathf.Lerp(_maxOutLineWidth, 0f, t);

            damageMaterial.SetFloat("_OutlineWidth", currentWidth);
            yield return null;
        }
        damageMaterial.SetFloat("_OutlineAlpha", 0f);
        
        _effectCoroutine = null;
    }
}