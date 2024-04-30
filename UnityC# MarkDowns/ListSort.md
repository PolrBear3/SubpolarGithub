# List.Sort
This list contians mixed integers.
```C#
List<int> myInts = new();
```

### Sorting the list from lowest to highest value
```C#
myInts.Sort((x, y) => x.CompareTo(y));
```

### Sorting the list from highest to lowest value
```C#
myInts.Sort((x, y) => y.CompareTo(x));
```

### Going through list +1 inside list length boundary
```C#
private List<Item> _items;
private Item currentItem;

private int currentArrayNum;

currentItem = _items[(currentArrayNum + 1) % _items.Length];
```