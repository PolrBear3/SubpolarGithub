using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TransitionCanvas_Controller : MonoBehaviour
{
    public static TransitionCanvas_Controller instance;
    
    
    [Header("")]
    [SerializeField] private RectTransform _curtain;
    [SerializeField] private Image _loadIconImage;

    [Header("")]
    [SerializeField] private LeanTweenType _leanTweenType;

    [Header("")]
    [SerializeField][Range(0F, 10F)] private float _loadTime;
    [SerializeField][Range(0F, 10F)] private float _transitionTime;
    
    
    private Coroutine _coroutine;
    public Coroutine coroutine => _coroutine;


    [Space(60)] 
    [SerializeField] private VideoGuide_Trigger _guideTrigger;
    

    // UnityEngine
    private void Awake()
    {
        instance = this;
    }
    
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
        _coroutine = StartCoroutine(CurrentScene_Transition_Coroutine());
    }
    private IEnumerator CurrentScene_Transition_Coroutine()
    {
        LeanTween.alpha(_curtain, 1f, 0f);
        LeanTween.moveX(_curtain, -1940f, 0);
        LeanTween.moveX(_curtain, 0f, _transitionTime).setEase(_leanTweenType);

        if (_loadIconImage != null)
        {
            LeanTween.alpha(_loadIconImage.rectTransform, 1f, 0.1f).setDelay(_transitionTime / 1.5f);
        }

        yield return new WaitForSeconds(_loadTime);
        
        Audio_Controller.instance.Play_OneShot(gameObject, 0);

        LeanTween.moveX(_curtain, -1940f, _transitionTime).setEase(_leanTweenType);
        LeanTween.alpha(_loadIconImage.rectTransform, 0f, 0.1f);
        
        _coroutine = null;
        yield break;
    }


    public void OpenScene_Transition()
    {
        _coroutine = StartCoroutine(OpenScene_Transition_Coroutine());
    }
    private IEnumerator OpenScene_Transition_Coroutine()
    {
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
        
        LeanTween.alpha(_curtain, 1f, 0f);
        LeanTween.moveX(_curtain, -1940f, _transitionTime).setEase(_leanTweenType);

        LeanTween.alpha(_loadIconImage.rectTransform, 0f, 0f);

        yield return new WaitForSeconds(_transitionTime);
        
        VideoGuide_Controller videoGuide = VideoGuide_Controller.instance;
        if (videoGuide == null)
        {
            _coroutine = null;
            yield break;
        }
        
        // guide
        VideoGuide_Controller.instance.Trigger_Guide(_guideTrigger);

        _coroutine = null;
        yield break;
    }

    public void CloseScene_Transition()
    {
        _coroutine = StartCoroutine(CloseScene_Transition_Coroutine());
    }
    private IEnumerator CloseScene_Transition_Coroutine()
    {
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
        
        LeanTween.alpha(_curtain, 1f, 0f);
        LeanTween.moveX(_curtain, 0f, _transitionTime).setEase(_leanTweenType); ;

        if (_loadIconImage != null)
        {
            LeanTween.alpha(_loadIconImage.rectTransform, 1f, 0.1f).setDelay(_transitionTime / 1.5f);
        }

        yield return new WaitForSeconds(_loadTime);
        
        _coroutine = null;
        yield break;
    }


    public void Toggle_PauseScreen(bool toggle)
    {
        if (Main_Controller.instance.globalVolume.profile.TryGet(out DepthOfField effect) == false) return;

        effect.active = toggle;
    }
}