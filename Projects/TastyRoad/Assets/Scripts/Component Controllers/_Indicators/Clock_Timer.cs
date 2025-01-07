using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock_Timer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;


    private Sprite[] _clockSprites;

    [Header("")]
    [SerializeField] private Sprite[] _defaultSprites;
    [SerializeField] private Sprite[] _greenSprites;


    private float _currentTime;
    public float currentTime => _currentTime;

    private int _timeBlockCount;
    public int timeBlockCount => _timeBlockCount;

    private Coroutine _timeCoroutine;
    private Coroutine _timeSpriteCoroutine;
    private Coroutine _toggleCoroutine;

    private bool _timeRunning;
    public bool timeRunning => _timeRunning;


    // MonoBehaviour
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _spriteRenderer = sr; }
    }

    private void Start()
    {
        _spriteRenderer.color = Color.clear;

        Toggle_ClockColor(false);
    }


    // Toggles
    public void Toggle_Transparency(bool isTransparent)
    {
        if (isTransparent)
        {
            _spriteRenderer.color = Color.clear;
            return;
        }

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


    public void Toggle_ClockColor(bool colorGreen)
    {
        if (colorGreen == false)
        {
            _clockSprites = _defaultSprites;
            return;
        }

        _clockSprites = _greenSprites;
    }


    // Time Control
    public void Set_Time(int setTime)
    {
        _currentTime = setTime;
        _timeBlockCount = _clockSprites.Length;
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

        _timeCoroutine = null;
        yield break;
    }

    private IEnumerator TimeSprite_Update_Coroutine()
    {
        float spriteTransitionTime = _currentTime / _clockSprites.Length;

        for (int i = 0; i < _clockSprites.Length; i++)
        {
            _spriteRenderer.sprite = _clockSprites[i];
            _timeBlockCount--;

            yield return new WaitForSeconds(spriteTransitionTime);
        }

        _timeSpriteCoroutine = null;
        yield break;
    }


    public void Stop_Time()
    {
        _timeRunning = false;

        if (_timeCoroutine != null) StopCoroutine(_timeCoroutine);
        if (_timeSpriteCoroutine != null) StopCoroutine(_timeSpriteCoroutine);

        _timeCoroutine = null;
        _timeSpriteCoroutine = null;
    }

    public void Update_CurrentTime(int updateTime)
    {
        _currentTime += updateTime;
    }
}