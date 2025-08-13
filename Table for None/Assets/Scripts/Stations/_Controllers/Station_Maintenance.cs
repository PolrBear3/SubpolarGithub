using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station_Maintenance : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Station_Controller _controller;

    [Header("")]
    [SerializeField] private GameObject _durabilityBreakIcon;


    public Action OnDurabilityBreak;


    // MonoBehaviour
    private void Start()
    {
        Update_DurabilityBreak();
    }


    // Durability Functions
    public void Update_Durability(int updateValue)
    {
        int calculatedDurability = _controller.data.durability + updateValue;
        
        _controller.data.Set_Durability(calculatedDurability);
    }
    
    public void Update_DurabilityBreak()
    {
        int currentDurability = _controller.data.durability;

        if (currentDurability > 1)
        {
            _durabilityBreakIcon.SetActive(false);
            return;
        }

        _durabilityBreakIcon.SetActive(true);

        if (currentDurability > 0) return;

        Main_Controller.instance.data.claimedPositions.Remove(transform.position);
        OnDurabilityBreak?.Invoke();

        Audio_Controller.instance.Play_OneShot(gameObject, 0);

        _controller.Destroy_Station();
    }
}
