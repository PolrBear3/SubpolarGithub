# Basic Inventory System

### It can
1. organizes item amount and leftover amount
2. returns leftover amount if all slots are full

### Item Scriptable Object
```C#
public class Item_ScrObj : ScriptableObject
{
    public string itemName;
    public int id;
    public int maxAmount;
}
```

### Slot
```C#
public class Slot
{
    private bool _hasItem;
    public bool hasItem => _hasItem;
    
    private Item_ScrObj _currentItem;
    public Item_ScrObj currentItem => _currentItem;
    
    private int _currentAmount;
    public int currentAmount => _currentAmount;

    public void Assign_Item(Item_ScrObj item, int amount)
    {
        _hasItem = true;
        
        _currentItem = item;
        _currentAmount = amount;
    }

    public void Update_Amount(int amount)
    {
        _currentAmount += amount;
    }
}
```

### Inventory
```C#
public class Inventory
{
    List<Slot> slots = new();

    public int Add_Item(Item_ScrObj item, int amount)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            // slot empty
            if (slots[i].hasItem == false)
            {
                if (amount > item.maxAmount)
                {
                    slots[i].Assign_Item(item, item.maxAmount);
                    Add_Item(item, amount - item.maxAmount);
                    return;
                }

                slots[i].Assign_Item(item, amount);
                return;
            }

            // slot not empty
            if (slots[i].currentItem != item) continue;
            if (slots[i].currentAmount >= item.maxAmount) continue;

            int updateAmount = slots[i].currentAmount + amount;

            if (updateAmount > item.maxAmount)
            {
                slots[i].Update_Amount(item.maxAmount);
                Add_Item(item, updateAmount - item.maxAmount);
                return;
            }

            slots[i].Update_Amount(amount);
            return;
        } 
    }
}
```