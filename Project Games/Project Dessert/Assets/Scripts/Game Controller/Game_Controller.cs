using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    public DataBase dataBase;
    public FieldCard_Track_System trackSystem;

    public Transform spawnPoint;
    [SerializeField] private float pushPowerRange;
    [SerializeField] private float powerDecreaseTime;
    [SerializeField] private float angleRange;

    private void Start()
    {
        StartCoroutine(Spawn_Default_Cards(1));
    }

    private IEnumerator Spawn_Default_Cards(float delayTime)
    {
        for (int i = 0; i < dataBase.defaultCards.Count; i++)
        {
            // instantiate defaultCards[i]
            var card = Instantiate(dataBase.defaultCards[i], spawnPoint);
            var cardController = card.GetComponent<Card_Controller>();

            // set random force power
            float randpower = Random.Range(1f, pushPowerRange);

            // set random angle
            float randAngle = Random.Range(-angleRange, angleRange);

            // add force to the right with set force power and angle
            cardController.detection.rb.AddForce(new Vector2(randAngle, -1f) * randpower, ForceMode2D.Impulse);
            StartCoroutine(Spawn_Push_Decrease(cardController.detection.rb));

            cardController.Update_Card(this, null, null);
            trackSystem.Addto_Track(cardController);

            yield return new WaitForSeconds(delayTime);
        }
    }
    private IEnumerator Spawn_Push_Decrease(Rigidbody2D rb)
    {
        float initialVelocity = rb.velocity.magnitude;
        float targetVelocity = 0f;
        float timeElapsed = 0f;

        while (timeElapsed < powerDecreaseTime)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / powerDecreaseTime;
            float currentVelocity = Mathf.Lerp(initialVelocity, targetVelocity, t);
            rb.velocity = new Vector2(currentVelocity, rb.velocity.y);
            yield return null;
        }

        // Make sure the velocity is exactly 0
        rb.velocity = Vector2.zero;
    }
}