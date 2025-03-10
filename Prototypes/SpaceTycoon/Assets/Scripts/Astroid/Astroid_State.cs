using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid_State : MonoBehaviour
{
    SpaceTycoon_Main_GameController controller;

    Rigidbody2D rb;
    SpriteRenderer sr;

    public Astroid_ScrObj[] Sector1Astroids;
    public Astroid_ScrObj[] Sector2Astroids;

    Astroid_ScrObj thisAstroid;
    float currentSpeed, setSpeed, currentRotationSpeed, SetRotationSpeed;
    
    int randomAstroidNum;
    int currentEnginesOn = 0;

    private void Awake()
    {
        controller = controller = GameObject.FindGameObjectWithTag("SpaceTycoon Main GameController").GetComponent<SpaceTycoon_Main_GameController>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Astroid_Start_Settings();
    }

    private void FixedUpdate()
    {
        Astroid_Movement();
    }

    private void Update()
    {
        Astroid_Speed_Conditions();
        Astroid_OutBound_Destory();
    }

    private void Astroid_Start_Settings()
    {
        if (controller.shipSectorLocation == 1)
        {
            randomAstroidNum = Random.Range(0, Sector1Astroids.Length);
            thisAstroid = Sector1Astroids[randomAstroidNum];
        }
        if (controller.shipSectorLocation == 2)
        {
            randomAstroidNum = Random.Range(0, Sector2Astroids.Length);
            thisAstroid = Sector2Astroids[randomAstroidNum];
        }

        sr.sprite = thisAstroid.astroidSprite;
    }

    private void Astroid_Movement()
    {
        rb.velocity = new Vector2(-1 * currentSpeed, rb.velocity.y);
        transform.rotation *= Quaternion.Euler(0, 0, currentRotationSpeed);
    }
    private void Astroid_Speed_Conditions()
    {
        // speed setting update
        if (currentEnginesOn < controller.EnginesOn)
        {
            setSpeed += thisAstroid.speedChangeValue;
            SetRotationSpeed += thisAstroid.rotationChangeValue;
            currentEnginesOn += 1;
        }
        else if (currentEnginesOn > controller.EnginesOn)
        {
            setSpeed -= thisAstroid.speedChangeValue;
            SetRotationSpeed -= thisAstroid.rotationChangeValue;
            currentEnginesOn -= 1;
        }

        // acceleration and deceleration update
        if (currentSpeed < setSpeed)
        {
            currentSpeed = currentSpeed + (thisAstroid.accelerationRate * Time.deltaTime);
        }
        else if (currentSpeed > setSpeed)
        {
            currentSpeed = currentSpeed - (thisAstroid.accelerationRate * Time.deltaTime);
        }

        if (currentRotationSpeed < SetRotationSpeed)
        {
            currentRotationSpeed = currentRotationSpeed + (thisAstroid.accelerationRate * Time.deltaTime);
        }
        else if (currentRotationSpeed > SetRotationSpeed)
        {
            currentRotationSpeed = currentRotationSpeed - (thisAstroid.accelerationRate * Time.deltaTime);
        }
    }

    private void Astroid_OutBound_Destory()
    {
        if (transform.position.x < -12.5f)
        {
            Destroy(gameObject);
        }
    }
}
