using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DragDrop_System : MonoBehaviour
{
    private Card_Controller controller;
    private BoxCollider2D boxCollider;

    public GameObject shadow;

    public SortingGroup mainSorting;

    [HideInInspector] public bool attached;

    public float followSpeed;
    public float shadowSpeed;
    public float shaodwOffset;

    private void Awake()
    {
        controller = gameObject.GetComponent<Card_Controller>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        Card_Clicked();
        Object_Lerp_Follow();
    }

    // card follow styles
    private void Object_Lerp_Follow()
    {
        if (!attached) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // y changing to x position in a delay
        transform.position = Vector2.Lerp(transform.position, mousePosition, Time.deltaTime * followSpeed);
    }
    private void Object_Attach_Detach()
    {
        if (attached)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
        }
    }

    // gameplay functions
    private void Card_Clicked()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (boxCollider != Physics2D.OverlapPoint(mousePosition)) return;

        if (!attached && !controller.detection.cardDetected) attached = true;
        else attached = false;

        SortingLayer_Update();
        Shadow();
    }

    // effects
    private void SortingLayer_Update()
    {
        if (attached)
        {
            mainSorting.sortingOrder = 1;
        }
        else if (!attached)
        {
            mainSorting.sortingOrder = 0;
        }
    }
    private void Shadow()
    {
        if (attached)
        {
            // alpha 30
            LeanTween.alpha(shadow, 0.2f, shadowSpeed);
            // local position
            LeanTween.moveLocal(shadow, new Vector2(0f, -0.7f), shadowSpeed);
            LeanTween.moveLocal(shadow, new Vector2(0f, -shaodwOffset), shadowSpeed);
            // scale 1
            LeanTween.scale(shadow, new Vector2(1f, 1f), shadowSpeed);
        }
        else
        {
            // alpha 0
            LeanTween.alpha(shadow, 0f, shadowSpeed);
            // local position 0, 0
            LeanTween.moveLocal(shadow, Vector2.zero, shadowSpeed);
            // scale 0
            LeanTween.scale(shadow, Vector2.zero, shadowSpeed);
        }
    }
}
