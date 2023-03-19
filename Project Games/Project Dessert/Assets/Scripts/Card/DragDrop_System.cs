using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DragDrop_System : MonoBehaviour
{
    private Card_Controller controller;
    private BoxCollider2D boxCollider;

    public GameObject shadow;
    public GameObject highlightBox;

    public SortingGroup mainSorting;

    [HideInInspector] public bool attached;

    public List<Card_Controller> highlightedCards = new List<Card_Controller>();

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

        Combine_to_ClosestCard();
        Highlight_Interactable_Cards();

        SortingLayer_Update();
        Shadow();
    }

    private void Highlight_Interactable_Cards()
    {
        var fieldCards = controller.controller.trackSystem.allFieldCards;

        for (int i = 0; i < fieldCards.Count; i++)
        {
            if (attached)
            {
                if (fieldCards[i] == controller) continue;
                if (!fieldCards[i].Combine_Check(controller)) continue;
                LeanTween.alpha(fieldCards[i].dragDrop.highlightBox, 0.7f, 0.25f);
            }
            else
            {
                LeanTween.alpha(fieldCards[i].dragDrop.highlightBox, 0f, 0.25f);
            }
        }
    }
    private void Combine_to_ClosestCard()
    {
        if (attached) return;

        List<Card_Controller> closeCards = new List<Card_Controller>();
        Card_Controller closestCard = null;

        if (highlightedCards.Count == 0) return;

        // highlighted cards that are detected to this card
        for (int i = 0; i < highlightedCards.Count; i++)
        {
            if (!controller.detection.DetectedCard_Match(highlightedCards[i])) continue;
            closeCards.Add(highlightedCards[i]);
        }

        if (closeCards.Count == 0) return;

        // find the closest card
        float smallestDistance = Mathf.Infinity;
        for (int i = 0; i < closeCards.Count; i++)
        {
            float cardDistance = Vector2.Distance(transform.position, closeCards[i].transform.position);

            if (cardDistance > smallestDistance) continue;

            smallestDistance = cardDistance;
            closestCard = closeCards[i];
        }

        closestCard.Increase_Amount(controller.data.currentAmount);
        controller.controller.trackSystem.Removefrom_Track(controller);
        Destroy(gameObject);
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
