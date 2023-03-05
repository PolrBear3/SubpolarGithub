using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DragDrop_System : MonoBehaviour
{
    private CardDetection_System detectionSystem;

    private BoxCollider2D boxCollider;

    public GameObject shadow;

    [HideInInspector] public bool attached;
    public float followSpeed;
    public float shadowSpeed;
    public float shaodwOffset;

    [SerializeField] private List<SortingGroup> sortings = new List<SortingGroup>();

    private void Awake()
    {
        if (gameObject.TryGetComponent(out CardDetection_System system))
        {
            detectionSystem = system;
        }

        if (gameObject.TryGetComponent(out BoxCollider2D system2))
        {
            boxCollider = system2;
        }
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
            Shadow();
        }
        else
        {
            attached = false;
            SortingLayer_Update();
            Shadow();
        }
    }

    // card follow styles
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
