using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class box : MonoBehaviour
{
    SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        AdjustCurrentHealth(0);

        BoxOpening();
        
        Destroyed();
    }

    public int currentHealth;
    public void AdjustCurrentHealth(int damage)
    {
        currentHealth += damage;
    }

    public Sprite open1_box;
    public Sprite open2_box;
    public Sprite open3_box;
    public Sprite open4_box;
    void BoxOpening()
    {
        if (currentHealth <= 80 && currentHealth > 60)
        {
            sr.sprite = open1_box;
        }
        else if (currentHealth <= 60 && currentHealth > 40)
        {
            sr.sprite = open2_box;
        }
        else if (currentHealth <= 40 && currentHealth > 20)
        {
            sr.sprite = open3_box;
        }
        else if (currentHealth <= 20)
        {
            sr.sprite = open4_box;
        }
    }

    public GameObject[] objects = new GameObject[0];
    public GameObject scatterPoint1, scatterPoint2, scatterPoint3;
    void Destroyed()
    {
        if (currentHealth <= 0)
        {
            Instantiate(objects[Random.Range (0, 4)], scatterPoint1.transform.position, scatterPoint1.transform.rotation);
            Instantiate(objects[Random.Range(0, 4)], scatterPoint2.transform.position, scatterPoint2.transform.rotation);
            Instantiate(objects[Random.Range(0, 4)], scatterPoint3.transform.position, scatterPoint3.transform.rotation);

            Destroy(gameObject);
        }
    }
}
