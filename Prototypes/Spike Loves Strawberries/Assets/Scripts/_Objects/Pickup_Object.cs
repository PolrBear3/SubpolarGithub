using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Object : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;
    
    [SerializeField] private Interact_Controller _interaction;
    public Interact_Controller interaction => _interaction;

    [Space(20)] 
    [SerializeField] [Range(0, 100)] private float _movementSpeed;
    [SerializeField] [Range(0, 10)] private float deceleratePower;
    
    
    private Coroutine _movementCoroutine;
    
    
    // MonoBehaviour
    public void Start()
    {
        Set_OnCurrentPlatform();
        
        // subscriptions
        _detection.OnPlayerDetect += Update_Indication;
        _detection.OnPlayerExit += Update_Indication;

        _interaction.OnInteract += Toggle_Pickup;
        _interaction.OnInteract += Update_Indication;
    }

    public void OnDestroy()
    {
        // subscriptions
        _detection.OnPlayerDetect -= Update_Indication;
        _detection.OnPlayerExit -= Update_Indication;
        
        _interaction.OnInteract -= Toggle_Pickup;
        _interaction.OnInteract -= Update_Indication;
    }
    
    
    // Indication
    public void Update_Indication()
    {
        Spike_Data data = Level_Controller.instance.player.data;
        bool isEmpty = data.hasItem == false && data.currentInteractable != gameObject;
        
        _interaction.Toggle_Indication(_detection.playerDetected && isEmpty);
    }
    
    
    // Pickup
    public void Toggle_Pickup()
    {
        Spike player = Level_Controller.instance.player;
        Spike_Data data = player.data;
        
        // drop
        if (data.hasItem && data.currentInteractable == gameObject)
        {
            data.Set_CurrentInteractable(null);

            Set_OnCurrentPlatform();
            Update_DestroyState();
            
            return;
        }

        // pickup
        if (data.hasItem) return;
        
        Cancel_Movement();
        
        data.Set_CurrentInteractable(gameObject);
        
        transform.SetParent(player.headTransform);
        transform.localPosition = Vector2.zero;
    }

    private void Update_DestroyState()
    {
        if (Level_Controller.instance.currentLevel.Position_OnPlatform(transform)) return;
        Destroy(gameObject);
    }


    public void Set_OnCurrentPlatform()
    {
        Level currentLevel = Level_Controller.instance.currentLevel;
        if (currentLevel.Position_OnPlatform(transform) == false) return;
        
        GameObject platform = currentLevel.Target_Platform(transform);
        transform.SetParent(platform.transform);
        transform.localRotation = Quaternion.identity;
    }
    
    
    // Movement
    private void Cancel_Movement()
    {
        if (_movementCoroutine == null) return;
        
        StopCoroutine(_movementCoroutine);
        _movementCoroutine = null;

        Update_DestroyState();
    }
    
    private bool IsWallBetween(Vector2 startPos, Vector2 endPos)
    {
        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;

        if (distance < 0.001f) return false;

        RaycastHit2D hit = Physics2D.Raycast(startPos, direction.normalized, distance, LayerMask.GetMask("Wall"));
        return hit.collider != null;
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
        
        Vector2 lastPosition = start;
        
        while (elapsed < duration)
        {
            float time = elapsed / duration;
            float easedTime = 1f - Mathf.Pow(1f - time, deceleratePower);
            Vector2 newPosition = Vector2.Lerp(start, targetPosition, easedTime);
            
            if (IsWallBetween(lastPosition, newPosition))
            {
                Cancel_Movement();
                yield break;
            }
            
            transform.position = newPosition;
            lastPosition = newPosition;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Cancel_Movement();
    }
}
