using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon_Controller : MonoBehaviour
{
    private SpriteRenderer _sr;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) _sr = sr;
    }

    //
    public void Assign(Sprite sprite)
    {
        if (sprite == null) return;

        _sr.color = Color.white;
        _sr.sprite = sprite;
    }

    public void Clear()
    {
        _sr.color = Color.clear;
        _sr.sprite = null;
    }
}
