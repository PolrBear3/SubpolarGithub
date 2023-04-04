using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag_Drop : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    
    private bool attached;
    public float followSpeed;
    
    private void Awake()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        Object_Clicked();
        Object_Attach_Detach();
    }

    private void Object_Clicked()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (boxCollider != Physics2D.OverlapPoint(mousePosition)) return;

        if (!attached) attached = true;
        else attached = false;
    }

    // original mouse follow
    private void Object_Attach_Detach()
    {
        if (attached)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = mousePosition;
        }
    }

    // smooth lerp follow
    private void Object_Lerp_Follow()
    {
        if (attached)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // y changing to x position in a delay
            transform.position = Vector2.Lerp(transform.position, mousePosition, Time.deltaTime * followSpeed);
        }
    }
}
