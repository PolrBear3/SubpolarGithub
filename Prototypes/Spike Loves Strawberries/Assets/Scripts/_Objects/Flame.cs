using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Detection_Controller _detection;

    [Space(20)] 
    [SerializeField][Range(0, 10)] private float _damageTime;
    
    
    private Coroutine _coroutine;
    
    
    // MonoBehaviour
    private void Start()
    {
        _detection.OnPlayerDetect += Damage_Player;
        _detection.OnPlayerExit += Cancel_PlayerDamage;
    }

    private void OnDestroy()
    {
        _detection.OnPlayerDetect -= Damage_Player;
        _detection.OnPlayerExit -= Cancel_PlayerDamage;
    }
    
    
    // Damage
    private void Cancel_PlayerDamage()
    {
        if (_coroutine == null) return;
        
        StopCoroutine(_coroutine);
        _coroutine = null;
    }
    
    private void Damage_Player()
    {
        Cancel_PlayerDamage();
        
        if (_detection.detectedPlayer == null) return;
        
        _coroutine = StartCoroutine(Damage_Player_Coroutine());
    }
    private IEnumerator Damage_Player_Coroutine()
    {
        yield return new WaitForSeconds(_damageTime);

        if (_detection.detectedPlayer == null)
        {
            _coroutine = null;
            yield break;
        }
        
        _detection.detectedPlayer.Update_Death();
        _coroutine = null;
    }
}
