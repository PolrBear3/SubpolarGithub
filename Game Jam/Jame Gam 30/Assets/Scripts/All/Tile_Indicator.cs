using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Indicator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [SerializeField] private GameObject _gearPlace;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) { _spriteRenderer = spriteRenderer; }
        if (gameObject.TryGetComponent(out Animator animator)) { _animator = animator; }
    }

    // Spin InActive
    public void Deactivate_Animation()
    {
        _spriteRenderer.color = Color.clear;
    }
    public void Spin_InActive_Animation(bool spinningRight)
    {
        _spriteRenderer.color = Color.white;
        _animator.SetBool("spinningRight", spinningRight);
    }

    // Gear
    public void GearPlace_Indication(bool activate)
    {
        if (activate) LeanTween.alpha(_gearPlace, 0.8f, 0f);
        else LeanTween.alpha(_gearPlace, 0f, 0f);
    }
}
