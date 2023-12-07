using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer_Clock : MonoBehaviour
{
    private SpriteRenderer _sr;

    public List<Sprite> clockSprites = new();

    public float setTime;
    public bool timeEnd;

    public Coroutine timeCoroutine;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
    }

    // Custom
    public void Reset_Time()
    {
        if (timeCoroutine != null) Stop_Time();

        _sr.sprite = clockSprites[0];
        timeEnd = false;
    }
    public void Stop_Time()
    {
        StopCoroutine(timeCoroutine);
    }

    public void Run_Time()
    {
        timeCoroutine = StartCoroutine(Time_Coroutine());
    }
    private IEnumerator Time_Coroutine()
    {
        float intervalTime = setTime / clockSprites.Count;
        int currentSpriteNum = 0;

        while(currentSpriteNum < clockSprites.Count)
        {
            yield return new WaitForSeconds(intervalTime);

            _sr.sprite = clockSprites[currentSpriteNum];
            currentSpriteNum++;
        }

        timeEnd = true;
    }
}
