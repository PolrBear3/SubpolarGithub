using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionCanvas_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private RectTransform _curtain;
    [SerializeField] private Image _loadIconImage;

    [Header("")]
    [SerializeField] private LeanTweenType _leanTweenType;

    [Header("")]
    [SerializeField][Range(0F, 10F)] private float _loadTime;
    [SerializeField][Range(0F, 10F)] private float _transitionTime;

    [Header("")]
    [SerializeField][Range(0f, 1f)] private float _pauseTransparency;


    public static bool transitionPlaying;


    private void Start()
    {
        OpenScene_Transition();
    }


    // Laod Icon
    public void Set_LoadIcon(Sprite sprite)
    {
        _loadIconImage.sprite = sprite;
    }


    // Transition Types
    public void CurrentScene_Transition()
    {
        StartCoroutine(CurrentScene_Transition_Coroutine());
    }
    private IEnumerator CurrentScene_Transition_Coroutine()
    {
        transitionPlaying = true;

        LeanTween.alpha(_curtain, 1f, 0f);
        LeanTween.moveX(_curtain, -1940f, 0);
        LeanTween.moveX(_curtain, 0f, _transitionTime).setEase(_leanTweenType);

        if (_loadIconImage != null)
        {
            LeanTween.alpha(_loadIconImage.rectTransform, 1f, 0.1f).setDelay(_transitionTime / 1.5f);
        }

        yield return new WaitForSeconds(_loadTime);

        LeanTween.moveX(_curtain, -1940f, _transitionTime).setEase(_leanTweenType);
        LeanTween.alpha(_loadIconImage.rectTransform, 0f, 0.1f);

        transitionPlaying = false;

        yield break;
    }


    public void OpenScene_Transition()
    {
        StartCoroutine(OpenScene_Transition_Coroutine());
    }
    private IEnumerator OpenScene_Transition_Coroutine()
    {
        transitionPlaying = true;

        LeanTween.alpha(_curtain, 1f, 0f);
        LeanTween.moveX(_curtain, -1940f, _transitionTime).setEase(_leanTweenType);

        LeanTween.alpha(_loadIconImage.rectTransform, 0f, 0f);

        yield return new WaitForSeconds(_transitionTime);
        transitionPlaying = false;

        yield break;
    }

    public void CloseScene_Transition()
    {
        StartCoroutine(CloseScene_Transition_Coroutine());
    }
    private IEnumerator CloseScene_Transition_Coroutine()
    {
        transitionPlaying = true;

        LeanTween.alpha(_curtain, 1f, 0f);
        LeanTween.moveX(_curtain, 0f, _transitionTime).setEase(_leanTweenType); ;

        if (_loadIconImage != null)
        {
            LeanTween.alpha(_loadIconImage.rectTransform, 1f, 0.1f).setDelay(_transitionTime / 1.5f);
        }

        yield return new WaitForSeconds(_loadTime);
        transitionPlaying = false;

        yield break;
    }


    public void Toggle_PauseScreen(bool toggle)
    {
        if (toggle)
        {
            LeanTween.alpha(_loadIconImage.rectTransform, 0f, 0f);

            _curtain.anchoredPosition = Vector2.zero;
            LeanTween.alpha(_curtain, _pauseTransparency, 0f);

            return;
        }

        LeanTween.alpha(_curtain, 0f, 0f);
    }
}