using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock_Timer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private List<Sprite> _clockSprites = new();
    [SerializeField] private Sprite _clockEmptySprite;

    private int _currentTime;
    private Coroutine _timeCoroutine;

    private bool _timeRunning;
    public bool timeRunning => _timeRunning;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _spriteRenderer = sr; }
    }

    private void Start()
    {
        _spriteRenderer.color = Color.clear;
    }



    // clock transparency toggle
    public void Toggle_Transparency(bool isTransparent)
    {
        if (isTransparent)
        {
            _spriteRenderer.color = Color.clear;
            return;
        }

        if (_currentTime <= 0 || _timeRunning == false) return;

        _spriteRenderer.color = Color.white;
    }



    // time control
    public void Set_Time(int setTime)
    {
        _currentTime = setTime;
    }

    public void Run_Time()
    {
        _timeRunning = true;
        _timeCoroutine = StartCoroutine(Run_Time_Coroutine());
    }
    private IEnumerator Run_Time_Coroutine()
    {
        int timePerSprite = _currentTime / _clockSprites.Count;

        while(_currentTime > 0)
        {
            _currentTime--;

            int spriteIndexNum = _currentTime / timePerSprite - 1;

            if (spriteIndexNum >= 0) 
            {
                _spriteRenderer.sprite = _clockSprites[spriteIndexNum];
            }

            yield return new WaitForSeconds(1);
        }

        _timeRunning = false;
        _spriteRenderer.sprite = _clockEmptySprite;

        Toggle_Transparency(true);
    }

    public void Stop_Time()
    {
        _timeRunning = false;

        if (_timeCoroutine == null) return;

        StopCoroutine(_timeCoroutine);
        _timeCoroutine = null;
    }



    /// <returns>
    /// Current sprite array number for time limit score.
    /// </returns>
    public int Current_TimeBlock_Amount()
    {
        for (int i = 0; i < _clockSprites.Count; i++)
        {
            if (_clockSprites[i] != _spriteRenderer.sprite) continue;
            return i + 1;
        }
        return 0;
    }
}