using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Object : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private Interact_Controller _interaction;

    [Space(20)] 
    [SerializeField] [Range(0, 100)] private float _movementSpeed;
    
    
    private Coroutine _movementCoroutine;
    
    
    // MonoBehaviour
    public void Start()
    {
        _detection.OnPlayerDetect += Update_Indication;
        _detection.OnPlayerExit += Update_Indication;

        _interaction.OnInteract += Toggle_Pickup;
        _interaction.OnInteract += Update_Indication;
    }

    public void OnDestroy()
    {
        _detection.OnPlayerDetect -= Update_Indication;
        _detection.OnPlayerExit -= Update_Indication;
        
        _interaction.OnInteract -= Toggle_Pickup;
        _interaction.OnInteract -= Update_Indication;
    }
    
    
    // Indication
    private void Update_Indication()
    {
        Spike_Data data = Level_Controller.instance.player.data;
        bool isEmpty = data.hasItem == false && data.currentInteractable != gameObject;
        
        _interaction.Toggle_Indication(_detection.playerDetected && isEmpty);
    }
    
    
    // Pickup
    private void Toggle_Pickup()
    {
        Spike player = Level_Controller.instance.player;
        Spike_Data data = player.data;
        
        // drop
        if (data.hasItem && data.currentInteractable == gameObject)
        {
            data.Set_CurrentInteractable(null);
            transform.SetParent(Level_Controller.instance.transform);
            transform.localRotation = Quaternion.identity;
            return;
        }

        // pickup
        if (data.hasItem) return;
        
        Cancel_Movement();
        
        data.Set_CurrentInteractable(gameObject);
        
        transform.SetParent(player.headTransform);
        transform.localPosition = Vector2.zero;
    }
    
    
    // Movement
    private void Cancel_Movement()
    {
        if (_movementCoroutine == null) return;
        
        StopCoroutine(_movementCoroutine);
        _movementCoroutine = null;
    }
    
    public void Trigger_PositionMovement(Vector2 targetPosition)
    {
        Cancel_Movement();
        _movementCoroutine = StartCoroutine(PositionMovement_Coroutine(targetPosition));
    }
    private IEnumerator PositionMovement_Coroutine(Vector2 targetPosition)
    {
        Vector2 start = transform.position;
        float distance = Vector2.Distance(start, targetPosition);
        
        float duration = distance / _movementSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector2.Lerp(start, targetPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        _movementCoroutine = null;
    }
}
