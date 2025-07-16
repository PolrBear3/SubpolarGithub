using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection_Controller : MonoBehaviour
{
    private BoxCollider2D _boxCollider;

    [SerializeField] private List<GameObject> _detectedprefabs;
    public List<GameObject> detectedprefabs => _detectedprefabs;


    private Player_Controller _player;
    public Player_Controller player => _player;
    
    public Action EnterEvent;
    public Action ExitEvent;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out BoxCollider2D boxCollider)) { _boxCollider = boxCollider; }
    }

    private void OnDestroy()
    {
        EnterEvent = null;
        ExitEvent = null;
    }


    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger == false) return;

        _detectedprefabs.Add(collision.gameObject);

        if (collision.TryGetComponent(out Player_Controller player) == false) return;

        _player = player;
        EnterEvent?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger == false) return;

        _detectedprefabs.Remove(collision.gameObject);

        if (collision.TryGetComponent(out Player_Controller player) == false) return;

        _player = null;
        ExitEvent?.Invoke();
    }


    /// <returns>Closest Detected Object</returns>
    public GameObject Closest_Object()
    {
        if (_detectedprefabs.Count <= 0) return null;

        float closestDistance = Vector2.Distance(_detectedprefabs[0].transform.position, transform.position);
        GameObject closestObject = _detectedprefabs[0];

        for (int i = 0; i < _detectedprefabs.Count; i++)
        {
            if (Vector2.Distance(_detectedprefabs[i].transform.position, transform.position) < closestDistance)
            {
                closestObject = _detectedprefabs[i];
            }
        }

        return closestObject;
    }

    /// <returns>All Detected prefabs that have IInteractable, distance sorted</returns>
    public List<IInteractable> All_Interactables()
    {
        List<(IInteractable interactable, float distance)> tempList = new();

        for (int i = 0; i < _detectedprefabs.Count; i++)
        {
            if (_detectedprefabs[i].TryGetComponent(out IInteractable interactable) == false) continue;

            float distance = Vector2.Distance(transform.position, _detectedprefabs[i].transform.position);
            tempList.Add((interactable, distance));
        }

        // Sort from closest to farthest
        tempList.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Extract just the sorted interactables
        List<IInteractable> returnList = new();
        foreach (var pair in tempList)
        {
            if (returnList.Contains(pair.interactable)) continue;
            returnList.Add(pair.interactable);
        }

        return returnList;
    }


    /// <returns>Closest Detected Object that has IInteractable</returns>
    public GameObject Closest_Interactable()
    {
        List<GameObject> detectedInteractables = new();

        for (int i = 0; i < _detectedprefabs.Count; i++)
        {
            if (_detectedprefabs[i].TryGetComponent(out IInteractable interactable))
            {
                detectedInteractables.Add(_detectedprefabs[i]);
            }
        }

        if (detectedInteractables.Count <= 0) return null;

        float closestDistance = Vector2.Distance(detectedInteractables[0].transform.position, transform.position);
        GameObject closestInteractable = detectedInteractables[0];

        for (int i = 0; i < detectedInteractables.Count; i++)
        {
            if (Vector2.Distance(detectedInteractables[i].transform.position, transform.position) < closestDistance)
            {
                closestInteractable = detectedInteractables[i];
            }
        }

        return closestInteractable;
    }


    // Detection Checks
    public bool Has_Object(GameObject specificObject)
    {
        for (int i = 0; i < _detectedprefabs.Count; i++)
        {
            if (specificObject == _detectedprefabs[i]) return true;
        }

        return false;
    }


    // Toggle Control
    public void Toggle_BoxCollider(bool toggleOn)
    {
        _boxCollider.enabled = toggleOn;
    }
}