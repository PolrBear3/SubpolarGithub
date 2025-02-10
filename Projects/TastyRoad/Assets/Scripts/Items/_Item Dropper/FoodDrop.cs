using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDrop : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private Detection_Controller _detection;

    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _destroyTikCount;
    [SerializeField][Range(0, 1)] private float _transparencyStep;
    private int _currentTikCount;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _foodIcon.SetMax_SubDataCount(_foodIcon.AllDatas().Count);
        AmountBar_Toggle();

        // subscriptions
        GlobalTime_Controller.instance.OnTimeTik += Activate_DestroyTimeTik;

        _detection.EnterEvent += AmountBar_Toggle;
        _detection.ExitEvent += AmountBar_Toggle;
    }

    private void OnDestroy()
    {
        // subscriptions
        GlobalTime_Controller.instance.OnTimeTik -= Activate_DestroyTimeTik;

        _detection.EnterEvent -= AmountBar_Toggle;
        _detection.ExitEvent -= AmountBar_Toggle;
    }


    // IInteractable
    public void Interact()
    {
        Pickup();
    }

    public void Hold_Interact()
    {
        Pickup_All();
    }

    public void UnInteract()
    {

    }


    // Food Pickup
    private void Pickup()
    {
        if (_foodIcon.hasFood == false)
        {
            Destroy(gameObject);
            return;
        }

        FoodData_Controller playerIcon = _detection.player.foodIcon;

        if (playerIcon.DataCount_Maxed()) return;

        ArchiveMenu_Controller archive = _detection.player.mainController.currentVehicle.menu.archiveMenu;

        archive.Toggle_DataLock(archive.Archive_Food(_foodIcon.currentData.foodScrObj), true);

        playerIcon.Set_CurrentData(_foodIcon.currentData);
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        _foodIcon.Set_CurrentData(null);
        _foodIcon.Toggle_SubDataBar(true);

        if (_foodIcon.hasFood) return;
        Destroy(gameObject);
    }

    private void Pickup_All()
    {
        if (_foodIcon.hasFood == false)
        {
            Destroy(gameObject);
            return;
        }

        FoodData_Controller playerIcon = _detection.player.foodIcon;
        int pickupAmount = _foodIcon.AllDatas().Count;

        for (int i = 0; i < pickupAmount; i++)
        {
            if (playerIcon.DataCount_Maxed()) return;
            Pickup();
        }
    }


    // Others
    private void AmountBar_Toggle()
    {
        _foodIcon.Toggle_SubDataBar(_detection.player != null);
    }

    private void Activate_DestroyTimeTik()
    {
        _currentTikCount++;
        Main_Controller.Change_SpriteAlpha(_sr, _sr.color.a - _transparencyStep);

        if (_currentTikCount < _destroyTikCount) return;
        Destroy(gameObject, 0.1f);
    }
}
