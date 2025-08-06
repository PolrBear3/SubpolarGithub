using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ClockTimer : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Image _clockImage;
    [SerializeField] private List<Sprite> _clockSprites = new();

    [Header("")]
    [SerializeField][Range(0, 10)] private float _inputHoldTime;

    private Coroutine _timeCoroutine;


    // UnityEngine
    private void Start()
    {
        Stop_ClockSpriteRun();
    }


    //
    public void Run_ClockSprite()
    {
        if (_timeCoroutine != null)
        {
            StopCoroutine(_timeCoroutine);
            _timeCoroutine = null;
        }

        _timeCoroutine = StartCoroutine(Run_ClockSprite_Coroutine());
    }
    private IEnumerator Run_ClockSprite_Coroutine()
    {
        float transitionTime = _inputHoldTime / _clockSprites.Count;

        for (int i = 0; i < _clockSprites.Count; i++)
        {
            yield return new WaitForSeconds(transitionTime);

            _clockImage.color = Color.white;
            _clockImage.sprite = _clockSprites[i];
        }
    }

    public void Stop_ClockSpriteRun()
    {
        if (_timeCoroutine != null)
        {
            StopCoroutine(_timeCoroutine);
            _timeCoroutine = null;
        }

        _clockImage.color = Color.clear;
    }
}
