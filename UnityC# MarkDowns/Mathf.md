# Mathf Returns

### Mathf.Abs
Returns the absolute value of a number
```C#
int absValue = Mathf.Abs(-1); // returns 1
```

### Mathf.Clamp
Sets the value inside the minimum and maximum values
```C#
// set value / minimum 0 / maximum 10
float minValue = Mathf.Clamp(-1, 0, 10); // 0
float maxValue = Mathf.Clamp(11, 0, 10); // 10
```

### Mathf.Lerp
Returns the point inside start and end values. A value of 0 corresponds to startValue, and a value of 1 corresponds to endValue. Values between 0 and 1 will give you values between startValue and endValue.
```C#
// start value / end value / point value
float midValue = Mathf.Lerp(0, 10, 0.5); // 5
float startValue = Mathf.Lerp(1, 10, 0); // 1
float endValue = Mathf.Lerp(0, 10, 1); // 10
```

### Mathf.Round
Rounds a float value to the nearest integer
```C#
float roundedValue1 = Mathf.Round(5.6f); // 6
float roundedValue2 = Mathf.Round(5.4f); // 4
```

### Mathf.Floor
Rounds a float value down
```C#
float roundedValue1 = Mathf.Floor(5.6f); // 5
float roundedValue2 = Mathf.Floor(5.4f); // 5
```

### Mathf.Ceil
Rounds a float value up
```C#
float roundedValue1 = Mathf.Ceil(5.6f); // 6
float roundedValue2 = Mathf.Ceil(5.4f); // 6
```