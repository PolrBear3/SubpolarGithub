using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection_Controller : MonoBehaviour
{
    [SerializeField] private List<GameObject> _detectedObjects;

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger == false) return;

        _detectedObjects.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger == false) return;

        _detectedObjects.Remove(collision.gameObject);
    }

    // Get Closest Detected Object
    public GameObject Closest_Object()
    {
        if (_detectedObjects.Count <= 0) return null;

        float closestDistance = Vector2.Distance(_detectedObjects[0].transform.position, transform.position);
        GameObject closestObject = _detectedObjects[0];

        for (int i = 0; i < _detectedObjects.Count; i++)
        {
            if (Vector2.Distance(_detectedObjects[i].transform.position, transform.position) < closestDistance)
            {
                closestObject = _detectedObjects[i];
            }
        }

        return closestObject;
    }

    // Get Specific Closest Detected Object
    public GameObject Closest_Object(List<GameObject> specificObjects)
    {
        if (_detectedObjects.Count <= 0) return null;

        float closestDistance = Vector2.Distance(specificObjects[0].transform.position, transform.position);
        GameObject closestObject = specificObjects[0];

        for (int i = 0; i < specificObjects.Count; i++)
        {
            if (Vector2.Distance(specificObjects[i].transform.position, transform.position) < closestDistance)
            {
                closestObject = specificObjects[i];
            }
        }

        return closestObject;
    }

    // Get Closest Interactable Detected Object
    public GameObject Closest_Interactable()
    {
        List<GameObject> detectedInteractables = new();

        for (int i = 0; i < _detectedObjects.Count; i++)
        {
            if (_detectedObjects[i].TryGetComponent(out IInteractable interactable))
            {
                detectedInteractables.Add(_detectedObjects[i]);
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
        for (int i = 0; i < _detectedObjects.Count; i++)
        {
            if (specificObject == _detectedObjects[i]) return true;
        }
        return false;
    }
}