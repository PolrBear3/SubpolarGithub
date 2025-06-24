using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Spike_Movement _movement;
    public Spike_Movement movement => _movement;
    
    [SerializeField] private Spike_Animation _anim;
    public Spike_Animation anim => _anim;

    [Space(20)] 
    [SerializeField][Range(0, 10)] private float _healCoolTime;
    public float healCoolTime => _healCoolTime;
    
    [SerializeField][Range(0, 10)] private int _maxDamageCount;
    [SerializeField][Range(0, 10)] private float _reviveCoolTime;

    [Space(20)] 
    [SerializeField] private GameObject _tailPrefab;
    
    [Space(20)] 
    [SerializeField] private Transform _headTransform;
    public Transform headTransform => _headTransform;
    
    [SerializeField] private Transform _tailTransform;
    
    
    private Spike_Data _data;
    public Spike_Data data => _data;

    private List<IInteractable> _detectedInteractables = new();
    
    private Coroutine _damageCoroutine;
    public Coroutine damageCoroutine => _damageCoroutine;
    
    private Coroutine _healCoroutine;

    public Action OnDamage;
    public Action OnDeath;
    
    
    // MonoBehaviour
    private void Awake()
    {
        _data = new(_maxDamageCount);
    }
    
    private void Start()
    {
        Main_InputSystem.instance.OnInteractInput += Interact_RecentInteractable;
    }
    
    private void OnDestroy()
    {
        Main_InputSystem.instance.OnInteractInput -= Interact_RecentInteractable;
    }
    
    
    // BoxCollider2D
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IInteractable interactable) == false) return;
        _detectedInteractables.Add(interactable);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IInteractable interactable) == false) return;
        _detectedInteractables.Remove(interactable);
    }
    
    
    // IInteractable
    private void Interact_RecentInteractable()
    {
        if (_detectedInteractables.Count == 0) return;
        _detectedInteractables[_detectedInteractables.Count - 1].Interact();
    }
    
    
    // Damage Control
    public void Toggle_TailDetachment(bool toggle)
    {
        _data.Toggle_HasTail(toggle);
        _anim.Update_AnimationOverride();

        if (toggle) return;
        Instantiate(_tailPrefab, _tailTransform.position, Quaternion.identity);
    }

    
    public void Cancel_Damage()
    {
        Update_Heal();
        
        if (_damageCoroutine == null) return;
        
        StopCoroutine(_damageCoroutine);
        _damageCoroutine = null;
    }
    
    public void Update_Damage()
    {
        if (_healCoroutine != null)
        {
            StopCoroutine(_healCoroutine);
            _healCoroutine = null;
        }

        if (_damageCoroutine != null) return;
        _damageCoroutine = StartCoroutine(Damage_Coroutine());
    }
    private IEnumerator Damage_Coroutine()
    {
        while (_data.damageCount > 1)
        {
            _data.Set_DamageCount(_data.damageCount - 1);
            OnDamage?.Invoke();
            
            yield return new WaitForSeconds(_healCoolTime);
        }
        Update_Death();
    }

    private void Update_Heal()
    {
        if (_data.damageCount >= _maxDamageCount) return;
        _healCoroutine = StartCoroutine(Heal_Coroutine());
    }
    private IEnumerator Heal_Coroutine()
    {
        yield return new WaitForSeconds(healCoolTime);
        
        data.Set_DamageCount(_maxDamageCount);
        yield break;
    }
    
    
    public void Update_Death()
    {
        StartCoroutine(Update_Death_Coroutine());
    }
    private IEnumerator Update_Death_Coroutine()
    {
        OnDeath?.Invoke();
        
        _movement.Toggle_Movement(false);
        _anim.sr.color = Color.clear;

        yield return new WaitForSeconds(_reviveCoolTime);

        _data.Set_DamageCount(_maxDamageCount);
        
        _anim.sr.color = Color.white;
        _movement.Toggle_Movement(true);

        // set revive position //
        transform.position = Vector2.zero;
    }
}
