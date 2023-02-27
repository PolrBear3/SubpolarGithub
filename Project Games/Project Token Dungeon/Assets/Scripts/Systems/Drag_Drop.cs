using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag_Drop : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private bool mouseAttached;
    public float followDelay;

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

        if (!mouseAttached) mouseAttached = true;
        else mouseAttached = false;
    }

    private void Object_Attach_Detach()
    {
        if (mouseAttached)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector2.Lerp(transform.position, mousePosition, Time.deltaTime * followDelay);
        }
    }
}
