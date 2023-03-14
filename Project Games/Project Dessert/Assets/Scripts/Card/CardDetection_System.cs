using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDetection_System : MonoBehaviour
{
    private Card_Controller controller;
    private Rigidbody2D rb;

    public GameObject detectionBox;

    [HideInInspector] public bool cardDetected;

    [SerializeField] private float pushSpeed;
    [SerializeField] private float pushDecreaseTime;

    [SerializeField] private List<Card_Controller> detectedCards = new List<Card_Controller>();

    private void Awake()
    {
        controller = gameObject.GetComponent<Card_Controller>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        Push_Cards();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Card_Controller other)) return;

        cardDetected = true;
        detectedCards.Add(other);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Card_Controller other)) return;

        cardDetected = false;
        detectedCards.Remove(other);

        StartCoroutine(Push_Decrease(rb));
    }

    private bool DetectedCard_Attached()
    {
        for (int i = 0; i < detectedCards.Count; i++)
        {
            if (detectedCards[i].dragDrop.attached) return true;
        }
        return false;
    }
    public void Push_Cards()
    {
        if (!cardDetected) return;
        if (controller.dragDrop.attached) return;
        if (DetectedCard_Attached()) return;

        // push left
        if (transform.position.x <= detectedCards[0].transform.position.x)
        {
            // transform.Translate(pushSpeed * Time.deltaTime * Vector2.left);
            rb.AddForce(Vector2.left * pushSpeed, ForceMode2D.Force);
        }
        // push right
        else if (transform.position.x > detectedCards[0].transform.position.x)
        {
            // transform.Translate(pushSpeed * Time.deltaTime * Vector2.right);
            rb.AddForce(Vector2.right * pushSpeed, ForceMode2D.Force);
        }
    }
    private IEnumerator Push_Decrease(Rigidbody2D rb)
    {
        float initialVelocity = rb.velocity.x;
        float targetVelocity = 0f;
        float timeElapsed = 0f;

        while (timeElapsed < pushDecreaseTime)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / pushDecreaseTime;
            float currentVelocity = Mathf.Lerp(initialVelocity, targetVelocity, t);
            rb.velocity = new Vector2(currentVelocity, rb.velocity.y);
            yield return null;
        }

        // Make sure the velocity is exactly 0
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    public void Highlight_Card(bool activation)
    {
        if (activation)
        {
            LeanTween.alpha(detectionBox, 0.7f, 0.25f);
        }
        else
        {
            LeanTween.alpha(detectionBox, 0f, 0.25f);
        }
    }
}