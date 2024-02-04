# IEnmerator

### Basics
```C#
private Coroutine myCoroutine

private IEnumerator Function_Coroutine()
{
    yield return null;
}

private void Start_Function()
{
    myCoroutine = StartCoroutine(Function_Coroutine());
}

private void Stop_Function()
{
    if (myCoroutine == null) return;

    StopCoroutine(myCoroutine);
}
```

### Wait For Seconds
Executes to the next part of the code after a set time has passed.

```C#
float waitTime;

private IEnumerator WaitTime_Coroutine()
{
    yield return new WaitForSeconds(waitTime);

    Debug.Log("This message will appear after waitTime!")
}
```

### Wait Until
Executes to the next part of the code until certain condition.

```C#
bool condition;

private IEnumerator WaitUntil_Coroutine()
{
    while (condition == true)
    {
        yield return null;
    }

    Debug.Log("This message will appear if condition is true!")
}
```