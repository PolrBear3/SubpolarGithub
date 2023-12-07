using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Movement : MonoBehaviour
{
    private SpriteRenderer _sr;

    private Customer_Controller _customerController;

    [Header("Data")]
    public float roamIntervalTime;
    public bool roamActive;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
        if (gameObject.TryGetComponent(out Customer_Controller customerController)) { _customerController = customerController; }
    }
    private void Start()
    {
        StartCoroutine(Free_Roam());
    }

    // Custom
    public void Flip_toPlayer()
    {
        Player_Controller player = _customerController.playerController;

        if (transform.position.x > player.transform.position.x) _sr.flipX = true;
        else _sr.flipX = false;
    }

    // Move Type
    public IEnumerator Free_Roam()
    {
        while (!roamActive)
        {
            yield return new WaitForSeconds(roamIntervalTime);
            Vector2 nextPos = _customerController.gameController.dataController.Get_BoxArea_Position(0);
            LeanTween.move(gameObject, nextPos, roamIntervalTime - .5f);
        }
    }
}