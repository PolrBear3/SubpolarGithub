using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ClockTimer : MonoBehaviour
{
    private Image _image;

    [SerializeField] private List<Sprite> _clockSprites = new();

    private const float _inputHoldTime = 1f;

    private Coroutine _timeCoroutine;


    // UnityEngine
    private void Awake()
    {
        _image = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        Stop_ClockSpriteRun();
    }



    //
    public void Run_ClockSprite()
    {
        _timeCoroutine = StartCoroutine(Run_ClockSprite_Coroutine());
    }
    private IEnumerator Run_ClockSprite_Coroutine()
    {
        float transitionTime = _inputHoldTime / _clockSprites.Count;

        for (int i = 0; i < _clockSprites.Count; i++) 
        {
            yield return new WaitForSeconds(transitionTime);

            _image.color = Color.white;
            _image.sprite = _clockSprites[i];
        }

        yield return new WaitForSeconds(transitionTime);

        _image.color = Color.clear;
    }

    public void Stop_ClockSpriteRun()
    {
        if (_timeCoroutine != null) StopCoroutine(_timeCoroutine);

        _image.color = Color.clear;
    }
}
