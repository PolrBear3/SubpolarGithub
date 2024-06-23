using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmountBar : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private Sprite[] _amountBarSprites;

    private bool _isTransparent;
    public bool isTransparent => _isTransparent;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }


    // Functions
    public void Load(int loadAmount)
    {
        _sr.sprite = _amountBarSprites[Mathf.Clamp(loadAmount, 0, _amountBarSprites.Length - 1)];
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
}
