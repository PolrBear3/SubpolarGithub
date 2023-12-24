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

    private BoxCollider2D _bc;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out BoxCollider2D bc)) { _bc = bc; }

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

    private IEnumerator Leave_Delay(float delayTime)
    {
        gameController.currentCharacters.Remove(gameObject);
        _bc.enabled = false;

        yield return new WaitForSeconds(delayTime);

        customerAnimation.Leave_Effect();
        yield return new WaitForSeconds(customerAnimation.alphaTime);

        gameController.spawnController.Spawn_Customer(1, delayTime * 2f);
        Destroy(gameObject);
    }
    public void Leave(float delayTime)
    {
        StartCoroutine(Leave_Delay(delayTime));
    }
}