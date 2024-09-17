using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock_Timer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private List<Sprite> _clockSprites = new();

    private float _currentTime;
    public float currentTime => _currentTime;

    private int _timeBlockCount;
    public int timeBlockCount => _timeBlockCount;

    private Coroutine _timeCoroutine;
    private Coroutine _timeSpriteCoroutine;
    private Coroutine _toggleCoroutine;

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

        if (_currentTime <= 0) return;

        _spriteRenderer.color = Color.white;
    }

    public void Toggle_Transparency(bool isTransparent, float delayTime)
    {
        if (_toggleCoroutine != null)
        {
            StopCoroutine(_toggleCoroutine);
            _toggleCoroutine = null;
        }

        _toggleCoroutine = StartCoroutine(Toggle_Transparency_Coroutine(isTransparent, delayTime));
    }
    private IEnumerator Toggle_Transparency_Coroutine(bool isTransparent, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Toggle_Transparency(isTransparent);
    }


    // time control
    public void Set_Time(int setTime)
    {
        _currentTime = setTime;
        _timeBlockCount = _clockSprites.Count;
    }


    public void Run_Time()
    {
        _timeRunning = true;

        _timeCoroutine = StartCoroutine(Run_Time_Coroutine());
        _timeSpriteCoroutine = StartCoroutine(TimeSprite_Update_Coroutine());
    }
    private IEnumerator Run_Time_Coroutine()
    {
        while (_currentTime > 1)
        {
            _currentTime--;
            yield return new WaitForSeconds(1);
        }

        _timeRunning = false;
        Toggle_Transparency(true);
    }

    private IEnumerator TimeSprite_Update_Coroutine()
    {
        float spriteTransitionTime = _currentTime / _clockSprites.Count;

        for (int i = 0; i < _clockSprites.Count; i++)
        {
            _spriteRenderer.sprite = _clockSprites[i];
            _timeBlockCount--;

            yield return new WaitForSeconds(spriteTransitionTime);
        }
    }


    public void Stop_Time()
    {
        _timeRunning = false;

        if (_timeCoroutine != null) StopCoroutine(_timeCoroutine);
        if (_timeSpriteCoroutine != null) StopCoroutine(_timeSpriteCoroutine);
    }


    public void Update_CurrentTime(int updateTime)
    {
        _currentTime += updateTime;
    }
}