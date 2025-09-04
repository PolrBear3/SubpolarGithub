using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene_Controller : MonoBehaviour
{
    public static Cutscene_Controller instance;

    
    [SerializeField] private PlayableDirector _playableDirector;
    public PlayableDirector playerdirector => _playableDirector;

    
    public Action OnPlay;
    public Action OnEnd;
    public Action<bool> OnToggle;

    private Coroutine _coroutine;
    public Coroutine coroutine => _coroutine;
    
    
    // MonoBehaviour
    private void Awake()
    {
        instance = this;

        _playableDirector.played += HandleDirectorPlayed;
        _playableDirector.stopped += HandleDirectorStopped;
    }

    private void OnDestroy()
    {
        _playableDirector.played -= HandleDirectorPlayed;
        _playableDirector.stopped -= HandleDirectorStopped;
    }
    
    
    // Actions
    private void HandleDirectorPlayed(PlayableDirector director)
    {
        OnToggle?.Invoke(false);
        OnPlay?.Invoke();
    }

    private void HandleDirectorStopped(PlayableDirector director)
    {
        OnToggle?.Invoke(true);
        OnEnd?.Invoke();
    }
    
    
    // Main Control
    public void Play_CurrentPlayable()
    {
        if (_coroutine != null) return;
        
        _playableDirector.Play();
        _coroutine = StartCoroutine(Play_Coroutine());
    }
    private IEnumerator Play_Coroutine()
    {
        while (_playableDirector.state == PlayState.Playing) yield return null;
        
        _playableDirector.Stop();
        _coroutine = null;
        
        OnToggle?.Invoke(true);
        OnEnd?.Invoke();
    }
}
