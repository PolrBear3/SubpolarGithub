using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    private Animator _anim;
    [SerializeField] private Animator _shadowAnim;

    private Player_Controller _playerController;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Animator anim)) { _anim = anim; }
        if (gameObject.TryGetComponent(out Player_Controller playerController)) _playerController = playerController;
    }
    private void Update()
    {
        Move_Animation();
    }

    //
    public void Move_Animation()
    {
        if (_playerController.playerMovement.rb.velocity == Vector2.zero)
        {
            _anim.SetBool("isMoving", false);
            _shadowAnim.SetBool("isMoving", false);
            return;
        }

        _anim.SetBool("isMoving", true);
        _shadowAnim.SetBool("isMoving", true);
    }
}
