using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection_Controller : MonoBehaviour
{
    [SerializeField] private List<GameObject> _detectedprefabs;

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger == false) return;

        _detectedprefabs.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger == false) return;

        _detectedprefabs.Remove(collision.gameObject);
    }

    // Get Closest Detected Object
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

    // Get Closest Interactable Detected Object
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

    // Check
    public bool Has_Object(GameObject specificObject)
    {
        for (int i = 0; i < _detectedprefabs.Count; i++)
        {
            if (specificObject == _detectedprefabs[i]) return true;
        }

        return false;
    }
}