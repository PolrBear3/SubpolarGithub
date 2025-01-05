using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmountBar : MonoBehaviour
{
    private SpriteRenderer _sr;

    [SerializeField] private Sprite[] _amountBarSprites;
    public Sprite[] amountBarSprite => _amountBarSprites;

    [Header("")]
    [SerializeField] private Sprite[] _defaultBarSprites;
    [SerializeField] private Sprite[] _greenBarSprites;

    private int _currentAmount;
    public int currentAmount => _currentAmount;

    private bool _toggledOn;
    public bool toggledOn => _toggledOn;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _maxAmount;
    public int maxAmount => _maxAmount;

    [Header("")]
    [SerializeField] private bool _toggleLocked;
    public bool toggleLocked => _toggleLocked;

    [SerializeField][Range(0, 100)] private float _durationTime;


    private Coroutine _amountBarCoroutine;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();

        Toggle(false);
        Load();
    }


    // Functions
    public void Set_Amount(int setAmount)
    {
        _currentAmount = setAmount;
        _currentAmount = Mathf.Clamp(_currentAmount, 0, _maxAmount);
    }

    public void Update_Amount(int updateAmount)
    {
        _currentAmount += updateAmount;
        _currentAmount = Mathf.Clamp(_currentAmount, 0, _maxAmount);
    }


    public void Set_MaxAmount(int setAmount)
    {
        _maxAmount = setAmount;
    }

    public bool Is_MaxAmount()
    {
        return _currentAmount >= _maxAmount;
    }


    public void Load()
    {
        Load(_currentAmount);
    }
    public void Load(int loadAmount)
    {
        int spriteIndex = Mathf.FloorToInt((float)loadAmount / _maxAmount * (_amountBarSprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, _amountBarSprites.Length - 1);

        _sr.sprite = _amountBarSprites[spriteIndex];
    }

    public void Load_Custom(int maxAmount, int currentAmount)
    {
        int spriteIndex = Mathf.FloorToInt((float)currentAmount / maxAmount * (_amountBarSprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, _amountBarSprites.Length - 1);

        _sr.sprite = _amountBarSprites[spriteIndex];
    }


    // Toggles
    public void Toggle_Lock(bool lockToggle)
    {
        _toggleLocked = lockToggle;
    }


    public void Toggle()
    {
        Toggle(!_toggledOn);
    }
    public void Toggle(bool toggleOn)
    {
        if (_toggleLocked)
        {
            _sr.color = Color.clear;
            return;
        }

        _toggledOn = toggleOn;

        if (_toggledOn)
        {
            _sr.color = Color.white;
            return;
        }

        _sr.color = Color.clear;
    }


    public void Toggle_Duration()
    {
        if (_amountBarCoroutine != null)
        {
            StopCoroutine(_amountBarCoroutine);
            _amountBarCoroutine = null;
        }

        _amountBarCoroutine = StartCoroutine(Toggle_Duration_Coroutine());
    }
    private IEnumerator Toggle_Duration_Coroutine()
    {
        Toggle(true);
        yield return new WaitForSeconds(_durationTime);

        Toggle(false);

        _amountBarCoroutine = null;
        yield break;
    }


    public void Toggle_BarColor(bool isColored)
    {
        if (isColored)
        {
            _amountBarSprites = _greenBarSprites;
            return;
        }

        _amountBarSprites = _defaultBarSprites;
    }
}
