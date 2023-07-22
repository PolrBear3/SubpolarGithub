using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini_ItemIcon : MonoBehaviour
{
    private SpriteRenderer _sr;

    private bool _hasItem;
    public bool hasItem { get => _hasItem; set => _hasItem = value; }

    private Item_ScrObj _currentItem;
    public Item_ScrObj currentItem { get => _currentItem; set => _currentItem = value; }

    [SerializeField] private int _currentAmount;
    public int currentAmount { get => _currentAmount; set => _currentAmount = value; }

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
    }

    // Get
    public int LeftOver()
    {
        if (currentItem == null) return 0;
        return _currentAmount - _currentItem.maxAmount;
    }

    // Check
    public bool Is_Max_Amount()
    {
        if (_currentAmount >= _currentItem.maxAmount) return true;
        else return false;
    }

    // Functions
    public void Assign_Item(Item_ScrObj item, int amount)
    {
        if (item == _currentItem)
        {
            _currentAmount += amount;
            return;
        }

        _hasItem = true;
        _currentItem = item;
        _sr.sprite = item.sprite;
        _currentAmount = amount;
    }
    public void Clear_Item()
    {
        _hasItem = false;
        _currentItem = null;
        _sr.sprite = null;
        _currentAmount = 0;
    }
}
