using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandEvents : MonoBehaviour
{
    [SerializeField] private Land _land;
    public Land land => _land;

    [Header("")]
    [SerializeField] private GameObject _eventAniamtor;
    [SerializeField] private Transform _eventAnimatorFilePoint;

    private List<EventAnimator> _currentAnimators = new();

    [Header("")]
    [SerializeField] private GameObject[] _allEvents;

    private Coroutine _eventAnimationCoroutine;


    // Functions
    public void Activate_AllEvents()
    {
        for (int i = 0; i < _allEvents.Length; i++)
        {
            if (!_allEvents[i].TryGetComponent(out ILandEventable landEvent)) continue;
            landEvent.Activate();
        }
    }


    // Event Visuals and Animations Control
    public void Play_CurrentEvents_Animation()
    {
        Update_RemovedEvents_Animation();

        if (_eventAnimationCoroutine != null) StopCoroutine(_eventAnimationCoroutine);
        _eventAnimationCoroutine = StartCoroutine(Play_AllEvent_Animation_Coroutine());
    }
    private IEnumerator Play_AllEvent_Animation_Coroutine()
    {
        List<EventScrObj> nonDuplicateEvents = new();

        for (int i = 0; i < _land.currentData.currentEvents.Count; i++)
        {
            if (nonDuplicateEvents.Contains(_land.currentData.currentEvents[i])) continue;
            nonDuplicateEvents.Add(_land.currentData.currentEvents[i]);
        }

        for (int i = 0; i < nonDuplicateEvents.Count; i++)
        {
            if (Has_EventAnimation(nonDuplicateEvents[i])) continue;

            GameObject spawnAnimator = Instantiate(_eventAniamtor, _eventAnimatorFilePoint);
            spawnAnimator.transform.SetParent(_eventAnimatorFilePoint);

            EventAnimator eventAnimator = spawnAnimator.GetComponent<EventAnimator>();
            _currentAnimators.Add(eventAnimator);
            eventAnimator.Play_EventAnimation(nonDuplicateEvents[i]);

            while (!AnimationClipFinished(eventAnimator.anim.GetCurrentAnimatorStateInfo(0)))
            {
                yield return null;
            }
        }
    }

    private void Update_RemovedEvents_Animation()
    {
        for (int i = _currentAnimators.Count - 1; i >= 0; i--)
        {
            if (_land.currentData.Has_Event(_currentAnimators[i].currentEvent)) continue;

            Destroy(_currentAnimators[i].gameObject);
            _currentAnimators.RemoveAt(i);
        }
    }


    public bool Has_EventAnimation(EventScrObj checkEvent)
    {
        for (int i = 0; i < _currentAnimators.Count; i++)
        {
            if (checkEvent != _currentAnimators[i].currentEvent) continue;
            return true;
        }
        return false;
    }

    private bool AnimationClipFinished(AnimatorStateInfo currentAnimation)
    {
        return currentAnimation.normalizedTime >= 1f;
    }
}