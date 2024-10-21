# Weighted Random Selection

Selects a random item according to the weight (high num = high percentage to be selected)

```C#
public struct Item
{
    public string itemName;
    public int itemWeight;
}
```

```C#
public class Item_Selector
{
    private Item[] _items;

    private Item Random_Item()
    {
        // get total wieght
        int totalWeight = 0;

        foreach (Item item in _items)
        {
            totalWeight += item.itemWeight;
        }

        // track values
        int randValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // get random food according to weight
        for (int i = 0; i < _items.Length; i++)
        {
            cumulativeWeight += _items[i].itemWeight;

            if (randValue >= cumulativeWeight) continue;

            return _items[i];
        }

        return null;
    }
}
```

# Fisher Yates Shuffle

```C#

```