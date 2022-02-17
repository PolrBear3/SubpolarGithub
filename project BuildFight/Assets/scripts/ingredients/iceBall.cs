using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iceBall : MonoBehaviour
{
    Animator anim;

    bool isHit;
    public int damage;

    float startTime = 0f;
    public float coolTime;
    public canAttack canAttack;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Input_Animation_andCoolDownn();
    }

    void Input_Animation_andCoolDownn()
    {
        if (Input.GetMouseButtonDown(0) && canAttack.enableAttack == true)
        {
            anim.SetTrigger("iceBall_Melee");
            startTime = Time.time;
            canAttack.enableAttack = false;
        }

        if (startTime + coolTime <= Time.time)
        {
            canAttack.enableAttack = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("box"))
        {
            if (isHit == false)
            {
                isHit = true;
                collision.GetComponent<box>().currentHealth -= damage;
            }
        }
        if (collision.CompareTag("wall"))
        {
            if (isHit == false)
            {
                isHit = true;
                collision.GetComponent<wall>().currentHealth -= damage;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("box"))
        {
            isHit = false;
        }
        if (collision.CompareTag("wall"))
        {
            isHit = false;
        }
    }
}
