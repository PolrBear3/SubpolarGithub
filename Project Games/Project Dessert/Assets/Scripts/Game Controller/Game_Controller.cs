using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    public DataBase dataBase;
    public FieldCard_Track_System trackSystem;

    [SerializeField] private float minPushPower;
    [SerializeField] private float maxPushPower;
    [SerializeField] private float angleRange;

    private void Start()
    {
        Spawn_DefaultCards();
    }

    private void Spawn_DefaultCards()
    {
        if (dataBase.defaultCards.Count == 0) return;

        StartCoroutine(Spawn_DefaultCards_Delay());
    }
    private IEnumerator Spawn_DefaultCards_Delay()
    {
        for (int i = 0; i < dataBase.defaultCards.Count; i++)
        {
            // instantiate defaultCards[i]
            var card = Instantiate(dataBase.defaultCards[i], new Vector2(0f, 6f), Quaternion.identity);
            // move spawn card to field cards point
            card.transform.parent = trackSystem.transform;
            
            // get card controller data
            if (!card.TryGetComponent(out Card_Controller cardController)) continue;

            // set random force power
            float randpower = Random.Range(minPushPower, maxPushPower);

            // set random angle
            float randAngle = Random.Range(-angleRange, angleRange);

            // add force to the right with set force power and angle
            cardController.detection.rb.AddForce(new Vector2(randAngle, -1f) * randpower, ForceMode2D.Impulse);
            StartCoroutine(Spawn_Push_Decrease(cardController.detection.rb));

            // update card and add to track system
            cardController.Update_Card(this, Card_Type.manager, 0);
            trackSystem.Addto_Track(cardController);

            yield return new WaitForSeconds(1f);
        }
    }
    private IEnumerator Spawn_Push_Decrease(Rigidbody2D rb)
    {
        while (rb.velocity.y <= 0)
        {
            rb.AddForce(-rb.velocity * 0.1f);
            yield return null;
        }

        rb.velocity = Vector2.zero;
    }
}