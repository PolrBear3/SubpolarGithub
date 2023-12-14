using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Controller : MonoBehaviour
{
    [HideInInspector] public Game_Controller gameController;
    [HideInInspector] public Player_Controller playerController;

    [HideInInspector] public Customer_Movement customerMovement;
    [HideInInspector] public Customer_Animation customerAnimation;
    [HideInInspector] public Customer_Order customerOrder;

    // UnityEngine
    private void Awake()
    {
        gameController = FindObjectOfType<Game_Controller>();

        if (gameObject.TryGetComponent(out Customer_Movement customerMovement)) { this.customerMovement = customerMovement; }
        if (gameObject.TryGetComponent(out Customer_Animation customerAnimation)) { this.customerAnimation = customerAnimation; }
        if (gameObject.TryGetComponent(out Customer_Order customerOrder)) { this.customerOrder = customerOrder; }
    }
    private void Start()
    {
        Spawn();
    }

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller playerController)) return;
        this.playerController = playerController;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller playerController)) return;
        this.playerController = null;
    }

    //
    public void Spawn()
    {
        gameController.Connect_Character(gameObject);
        customerAnimation.Spawn_Effect();
    }
}