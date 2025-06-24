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
    [SerializeField] private float _deathTime;
    public float deathTime => _deathTime;

    [Space(60)] 
    [SerializeField] private Transform _testSpawnPoit;
    
    
    private Spike_Data _data;
    public Spike_Data data => _data;

    private List<IInteractable> _detectedInteractables = new();
    
    
    // MonoBehaviour
    private void Awake()
    {
        _data = new();
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
    
    
    // Control
    public void Update_Death()
    {
        StartCoroutine(Update_Death_Coroutine());
    }
    private IEnumerator Update_Death_Coroutine()
    {
        _movement.Toggle_Movement(false);
        _anim.sr.color = Color.clear;

        yield return new WaitForSeconds(_deathTime);
        
        movement.Toggle_Movement(true);
        _anim.sr.color = Color.white;

        // set revive position //
        transform.position = _testSpawnPoit.position;
    }
}
