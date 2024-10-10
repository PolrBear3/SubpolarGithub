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

    private bool _isTransparent;
    public bool isTransparent => _isTransparent;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }


    // Functions
    public void Set_Amount(int setAmount)
    {
        _currentAmount = setAmount;

        if (_currentAmount >= 0) return;

        _currentAmount = 0;
    }

    public void Update_Amount(int updateAmount)
    {
        _currentAmount += updateAmount;

        if (_currentAmount >= 0) return;

        _currentAmount = 0;
    }


    public bool Is_MaxAmount()
    {
        return _currentAmount >= _amountBarSprites.Length;
    }


    public void Load()
    {
        Load(_currentAmount);
    }
    public void Load(int loadAmount)
    {
        _sr.sprite = _amountBarSprites[Mathf.Clamp(loadAmount, 0, _amountBarSprites.Length - 1)];
    }

    public void Load_Custom(int maxAmount, int currentAmount)
    {
        int spriteIndex = Mathf.FloorToInt((float)currentAmount / maxAmount * (_amountBarSprites.Length - 1));

        spriteIndex = Mathf.Clamp(spriteIndex, 0, _amountBarSprites.Length - 1);
        _sr.sprite = _amountBarSprites[spriteIndex];
    }


    public void Transparent_Toggle()
    {
        Transparent_Toggle(!_isTransparent);
    }
    public void Transparent_Toggle(bool toggleOn)
    {
        _isTransparent = toggleOn;

        if (_isTransparent)
        {
            _sr.color = Color.clear;
        }
        else
        {
            _sr.color = Color.white;
        }
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
