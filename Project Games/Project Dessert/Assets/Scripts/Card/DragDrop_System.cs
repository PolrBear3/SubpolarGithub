using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DragDrop_System : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private bool attached;
    public float followSpeed;

    [SerializeField] private List<SortingGroup> sortings = new List<SortingGroup>();

    private void Awake()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        Object_Clicked();
        Object_Lerp_Follow();
    }

    private void Object_Clicked()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (boxCollider != Physics2D.OverlapPoint(mousePosition)) return;

        if (!attached) 
        {
            attached = true;
            SortingLayer_Update();
        }
        else
        {
            attached = false;
            SortingLayer_Update();
        }
    }

    private void Object_Lerp_Follow()
    {
        if (attached)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // y changing to x position in a delay
            transform.position = Vector2.Lerp(transform.position, mousePosition, Time.deltaTime * followSpeed);
        }
    }
    private void Object_Attach_Detach()
    {
        if (attached)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = mousePosition;
        }
    }

    private void SortingLayer_Update()
    {
        for (int i = 0; i < sortings.Count; i++)
        {
            if (attached)
            {
                sortings[i].sortingOrder += 1;
            }
            else
            {
                sortings[i].sortingOrder -= 1;
            }
        }
    }
}
