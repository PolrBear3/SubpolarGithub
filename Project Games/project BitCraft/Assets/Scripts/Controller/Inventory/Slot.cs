using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    
    private Inventory_Controller _inventory;

    private void Awake()
    {
        if (gameObject.TryGetComponent(out Image image)) { itemImage = image; }
    }

    public void Set_Inventory_Controller(Inventory_Controller inventory)
    {
        _inventory = inventory;
    }
}