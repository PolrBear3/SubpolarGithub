using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    private SpriteRenderer _sr;

    private NPC_Controller _controller;

    private Vector2 _targetPoint;

    [SerializeField] private float _moveSpeed;

    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _controller = gameObject.GetComponent<NPC_Controller>();
    }

    private void Update()
    {
        if (Is_TargetPoint())
        {
            _controller.animationControl.Play_AnimationState("NPC_idle");
            return;
        }

        if (_controller.interaction.arrested)
        {
            _controller.animationControl.Play_AnimationState("NPC_arrest");
        }
        else
        {
            _controller.animationControl.Play_AnimationState("NPC_walk");
        }

        Move();
    }

    // Checks
    public bool Is_TargetPoint()
    {
        float distance = Vector2.Distance(transform.position, _targetPoint);

        if (distance < 0.1f) return true;
        else return false;
    }

    // Update Movement
    public void Move()
    {
        // transform.Translate(_moveSpeed * Time.deltaTime * new Vector2(1f, 0f));
        transform.position = Vector2.MoveTowards(transform.position, _targetPoint, _moveSpeed * Time.deltaTime);
    }

    //
    public void Set_TargetPoint(Vector2 point)
    {
        _targetPoint = point;
    }



    //
    public void Leave()
    {
        StartCoroutine(Leave_Coroutine());
    }
    private IEnumerator Leave_Coroutine()
    {
        _sr.flipX = true;

        Set_TargetPoint(_controller.gameController.spawnPoint.position);

        _controller.gameController.UnTrack_NPC(_controller);
        _controller.currentSection.UnTrack_NPC(_controller);

        _controller.currentSection.Line_NPCs();

        while (Is_TargetPoint() == false)
        {
            yield return null;
        }

        _controller.interaction.LeaveScore_Update();

        Destroy(gameObject);
    }
}
