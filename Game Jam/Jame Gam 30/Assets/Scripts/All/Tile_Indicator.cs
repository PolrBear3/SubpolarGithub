using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Indicator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [SerializeField] private GameObject _gearIndicator;
    [SerializeField] private SpriteRenderer _goldGearIndicator;
    [SerializeField] private SpriteRenderer _objectGearIndicator;

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
    public void Gear_Indication(bool activate)
    {
        if (activate) LeanTween.alpha(_gearIndicator, 0.8f, 0f);
        else LeanTween.alpha(_gearIndicator, 0f, 0f);
    }

    // Gold Gear
    public void GoldGear_Indication()
    {
        _goldGearIndicator.color = Color.white;
    }

    // Object Gear
    public void ObjectGear_Indication()
    {
        _objectGearIndicator.color = Color.white;
    }
}
