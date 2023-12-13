using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Movement : MonoBehaviour
{
    private SpriteRenderer _sr;
    [HideInInspector] public Rigidbody2D rb;
    private Customer_Controller _customerController;

    [Header("Data")]
    public float moveSpeed;
    [HideInInspector] public Vector2 updatePosition;

    [Header("Free Roam")]
    public float roamIntervalTime;
    public float roamDelayTime;
    public bool roamActive;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
        if (gameObject.TryGetComponent(out Rigidbody2D rb)) { this.rb = rb; }
        if (gameObject.TryGetComponent(out Customer_Controller customerController)) { _customerController = customerController; }
    }
    private void Start()
    {
        StartCoroutine(Free_Roam());
    }
    private void Update()
    {
        UpdatePosition_Movement();
    }

    // Check
    public bool Is_RoamActive()
    {
        if ((Vector2)transform.position == updatePosition || !roamActive) return false;
        return true;
    }

    // Custom
    private void UpdatePosition_Movement()
    {
        if (!Is_RoamActive()) return;
        transform.position = Vector2.MoveTowards(transform.position, updatePosition, .5f * Time.deltaTime);
    }

    // Flip
    public void Flip_toPlayer()
    {
        Player_Controller player = _customerController.playerController;

        if (transform.position.x > player.transform.position.x) _sr.flipX = true;
        else _sr.flipX = false;
    }
    private void Flip_Update()
    {
        if (transform.position.x > updatePosition.x) _sr.flipX = true;
        else _sr.flipX = false;
    }

    // Move Type
    public IEnumerator Free_Roam()
    {
        updatePosition = transform.position;

        yield return new WaitForSeconds(roamDelayTime);

        roamActive = true;

        while (roamActive)
        {
            yield return new WaitForSeconds(roamIntervalTime);

            Vector2 nextPos = _customerController.gameController.dataController.Get_BoxArea_Position(0);
            updatePosition = nextPos;

            Flip_Update();
        }
    }
    public void Stop_Roam()
    {
        roamActive = false;
        updatePosition = transform.position;
    }
}