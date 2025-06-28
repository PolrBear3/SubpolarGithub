using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _boxCollider;
    public BoxCollider2D boxCollider => _boxCollider;
    
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
    public float reviveCoolTime => _reviveCoolTime;

    [Space(20)] 
    [SerializeField] private GameObject _tailPrefab;
    
    [Space(20)] 
    [SerializeField] private Transform _headTransform;
    public Transform headTransform => _headTransform;
    
    [SerializeField] private Transform _tailTransform;


    private bool _isDead;
    public bool isDead => _isDead;
    
    private Spike_Data _data;
    public Spike_Data data => _data;

    private Interact_Controller _releaseInteractable;

    
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
        Main_InputSystem inputSystem = Main_InputSystem.instance;
        
        inputSystem.OnInteractInput += Interact_CurrentInteractable;
        inputSystem.OnInteractRelease += Invoke_InteractRelease;

        OnDeath += Invoke_InteractRelease;
    }

    private void FixedUpdate()
    {
        FallDeath_Update();
    }
    
    private void OnDestroy()
    {
        Main_InputSystem inputSystem = Main_InputSystem.instance;
        
        inputSystem.OnInteractInput -= Interact_CurrentInteractable;
        inputSystem.OnInteractRelease -= Invoke_InteractRelease;
        
        OnDeath -= Invoke_InteractRelease;
    }
    
    
    // BoxCollider2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IInteractable interactable) == false) return;
        _data.detectedInteractables.Add(other.gameObject);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IInteractable interactable) == false) return;
        _data.detectedInteractables.Remove(other.gameObject);
    }
    

    // Interact
    private List<GameObject> HeadClosest_InteractObjects()
    {
        List<GameObject> detectedInteractables = _data.detectedInteractables;

        detectedInteractables.Sort((a, b) =>
        {
            float distA = Vector2.SqrMagnitude(a.transform.position - _headTransform.position);
            float distB = Vector2.SqrMagnitude(b.transform.position - _headTransform.position);
            return distA.CompareTo(distB);
        });
        return detectedInteractables;
    }
    
    private void Interact_CurrentInteractable()
    {
        if (_isDead) return;
        
        List<GameObject> interactObjects = HeadClosest_InteractObjects();

        for (int i = 0; i < interactObjects.Count; i++)
        {
            if (_data.hasItem && interactObjects[i] == _data.currentInteractable) continue;
            if (interactObjects[i].TryGetComponent(out IInteractable interactable) == false) continue;
            
            interactable.Interact();
            return;
        }

        if (_data.hasItem == false) return;
        if (_data.currentInteractable.TryGetComponent(out IInteractable currentObject) == false) return;
        
        currentObject.Interact();
    }
    
    
    // Interact Release
    public void Set_ReleaseInteractable(Interact_Controller interactObject)
    {
        _releaseInteractable = interactObject;
    }

    private void Invoke_InteractRelease()
    {
        if (_releaseInteractable == null) return;
        
        _releaseInteractable.OnInteractRelease?.Invoke();
    }
    
    
    // Damage Control
    public void Toggle_TailDetachment(bool toggle)
    {
        _data.Toggle_HasTail(toggle);
        _anim.Update_AnimationOverride();

        if (toggle) return;
        
        GameObject tail = Instantiate(_tailPrefab, _tailTransform.position, Quaternion.identity);
        GameObject platform = Level_Controller.instance.currentLevel.Target_Platform(tail.transform);
        
        tail.transform.SetParent(platform.transform);
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
        if (_isDead) return;
        
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
        
        if (_data.hasItem == false) return;
        
        Destroy(_data.currentInteractable);
        _data.Set_CurrentInteractable(null);
    }
    private IEnumerator Update_Death_Coroutine()
    {
        _isDead = true;
        OnDeath?.Invoke();
        
        _movement.Toggle_Movement(false);
        
        yield return new WaitForSeconds(_reviveCoolTime);
        
        _isDead = false;
        _data.Set_DamageCount(_maxDamageCount);
        
        transform.position = Level_Controller.instance.currentLevel.spawnPoint.position;
        
        _movement.Toggle_Movement(true);
    }


    private void FallDeath_Update()
    {
        if (_isDead) return;
        if (Level_Controller.instance.currentLevel.Position_OnPlatform(transform)) return;
        
        StartCoroutine(FallDeath_Coroutine());
        Update_Death();
    }
    private IEnumerator FallDeath_Coroutine()
    {
        _anim.sr.color = Color.clear;
        
        yield return new WaitForSeconds(_reviveCoolTime);
       
        _anim.sr.color = Color.white;
    }
}
