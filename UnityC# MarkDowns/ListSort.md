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