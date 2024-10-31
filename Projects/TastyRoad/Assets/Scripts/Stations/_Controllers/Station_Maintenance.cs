using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station_Maintenance : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Station_Controller _controller;

    [Header("")]
    [SerializeField] private GameObject _durabilityBreakIcon;


    public delegate void MaintenenceHandler();
    public event MaintenenceHandler OnDurabilityBreak;


    // MonoBehaviour
    private void Start()
    {
        Update_DurabilityBreak();
    }


    // Durability Functions
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

        OnDurabilityBreak?.Invoke();
        _controller.Destroy_Station();
    }
}
