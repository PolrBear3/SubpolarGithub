using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateAnimation
{
    public FoodState_Type type;
    public AnimatorOverrideController animOverrideController;
}

public class StateAnimationIcon_Controller : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private List<StateAnimation> stateAnimations = new();

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Animator animator)) { _animator = animator; }
    }

    private void Start()
    {
        Toggle_Transparency(false);
    }

    // Play State Type Animation
    public void Assign_Animation(FoodState_Type type)
    {
        for (int i = 0; i < stateAnimations.Count; i++)
        {
            if (stateAnimations[i].type != type) continue;
            if (stateAnimations[i].animOverrideController == null) return;

            _animator.runtimeAnimatorController = stateAnimations[i].animOverrideController;
        }
    }
    
    // Transparency Control
    public void Toggle_Transparency(bool toggleOn)
    {
        if (toggleOn)
        {
            LeanTween.alpha(gameObject, 1f, 0.01f);
        }
        else
        {
            LeanTween.alpha(gameObject, 0f, 0.01f);
        }
    }
}