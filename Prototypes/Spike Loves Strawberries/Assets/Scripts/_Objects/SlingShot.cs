using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private Interact_Controller _interaction;

    [Space(20)] 
    [SerializeField] private GameObject _sling;

    [Space(20)] 
    [SerializeField][Range(0, 10)] private float _distanceMultiplier;

    [Space(20)] 
    [SerializeField] private LeanTweenType _tweenType;
    [SerializeField] [Range(0, 10)] private float _slingSpeed;


    private bool _aiming;
    private Pickup_Object _placedObject;
    
    
    // MonoBehaviour
    private void Start()
    {
        _detection.OnPlayerDetect += Update_Indication;
        _detection.OnPlayerExit += Update_Indication;

        _interaction.OnInteract += Aim;
        _interaction.OnInteract += Update_Indication;
        
        _interaction.OnInteractRelease += Release;
    }

    private void OnDestroy()
    {
        _detection.OnPlayerDetect -= Update_Indication;
        _detection.OnPlayerExit -= Update_Indication;
        
        _interaction.OnInteract -= Aim;
        _interaction.OnInteract -= Update_Indication;

        _interaction.OnInteractRelease -= Release;
    }


    // Indication
    private void Update_Indication()
    {
        bool toggle = Level_Controller.instance.player.data.hasItem && _aiming == false;
        _interaction.Toggle_Indication(toggle);
    }
    
    
    // Interaction
    private void Aim()
    {
        if (_detection.playerDetected == false) return; 
        
        Spike player = Level_Controller.instance.player;
        Spike_Data playerData = player.data;
        
        if (playerData.hasItem == false) return;
        if (playerData.currentInteractable.TryGetComponent(out Pickup_Object pickupObject) == false) return;

        player.Set_ReleaseInteractable(_interaction);
        
        _aiming = true;
        _placedObject = pickupObject;
        
        _sling.transform.SetParent(player.headTransform);

        LeanTween.moveLocal(_sling, Vector2.zero, _slingSpeed).setEase(_tweenType);
        LeanTween.rotateLocal(_sling, new Vector3(0f, 0f, 180f), 0.2f).setEase(_tweenType);

        Audio_Controller.instance.Create_EventInstance(gameObject, 0).start();
    }

    private void Release()
    {
        Spike player = Level_Controller.instance.player;
       
        player.data.Set_CurrentInteractable(null);
        _placedObject.transform.SetParent(Level_Controller.instance.transform);
        _placedObject.transform.localRotation = Quaternion.identity;
        
        Vector2 direction = _sling.transform.up;
        float pullDistance = Vector2.Distance(_sling.transform.position, transform.position);
        
        Vector2 targetPos = (Vector2)_sling.transform.position + direction * pullDistance * _distanceMultiplier;
        _placedObject.Trigger_PositionMovement(targetPos);
        
        player.Set_ReleaseInteractable(null);
        Reset_Sling();

        Audio_Controller audio = Audio_Controller.instance;
        
        audio.EventInstance(gameObject, 0).stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        audio.Remove_EventInstance(gameObject, 0);
        
        audio.Play_OneShot(gameObject, 1);
    }

    private void Reset_Sling()
    {
        _aiming = false;
        _placedObject = null;
        
        // lean tween bounce back effect //
        _sling.transform.SetParent(transform);
        
        LeanTween.moveLocal(_sling, Vector2.zero, _slingSpeed).setEase(_tweenType);
        LeanTween.rotateLocal(_sling, Vector3.zero, _slingSpeed).setEase(_tweenType);
    }
}
