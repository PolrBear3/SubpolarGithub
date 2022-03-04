using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_GroundObject : MonoBehaviour
{
    private float snapRange = 0.1f;
    
    private bool isMoving;
    private bool snapped = false;
    private bool facingLeft = false;

    private float startPosX;
    private float startPosY;

    private Vector3 resetPos;

    void Start()
    {
        resetPos = this.transform.localPosition;
    }

    void Update()
    {
        Moving_Check();
        Drag_Flip();
    }

    void Moving_Check()
    {
        if (snapped == false)
        {
            if (isMoving)
            {
                Vector3 mousePos;
                mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                this.gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY,
                this.gameObject.transform.localPosition.z);
            }
        }
    }

    void Drag_Flip()
    {
        if (isMoving && Input.GetKeyDown(KeyCode.R))
        {
            Flip();
        }
    }
    void Flip()
    {
        facingLeft = !facingLeft;
        transform.Rotate(0, 180f, 0f);
    }

    void Reset_Position()
    {
        this.transform.localPosition = new Vector3(resetPos.x, resetPos.y, resetPos.z);
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            startPosX = mousePos.x - this.transform.localPosition.x;
            startPosY = mousePos.y - this.transform.localPosition.y;

            isMoving = true;

            // snapPoint indication mark
            allSnapPoints.SetActive(true);
        }
    }

    public GameObject allSnapPoints;
    public GameObject snapPoint1;
    public GameObject snapPoint2;
    public GameObject snapPoint3;
    public GameObject snapPoint4;
    public GameObject snapPoint5;
    public GameObject snapPoint6;
    public GameObject snapPoint7;

    private void OnMouseUp()
    {
        isMoving = false;

        // snapPoint indication mark
        allSnapPoints.SetActive(false);

        // snapPoint1
        if (Mathf.Abs(this.transform.localPosition.x - snapPoint1.transform.localPosition.x) <= snapRange &&
            Mathf.Abs(this.transform.localPosition.y - snapPoint1.transform.localPosition.y) <= snapRange)
        {
            this.transform.localPosition = new Vector2(snapPoint1.transform.localPosition.x, snapPoint1.transform.localPosition.y);
            snapped = true;
        }
        // snapPoint2
        else if (Mathf.Abs(this.transform.localPosition.x - snapPoint2.transform.localPosition.x) <= snapRange &&
            Mathf.Abs(this.transform.localPosition.y - snapPoint2.transform.localPosition.y) <= snapRange)
        {
            this.transform.localPosition = new Vector2(snapPoint2.transform.localPosition.x, snapPoint2.transform.localPosition.y);
            snapped = true;
        }
        // snapPoint3
        else if (Mathf.Abs(this.transform.localPosition.x - snapPoint3.transform.localPosition.x) <= snapRange &&
            Mathf.Abs(this.transform.localPosition.y - snapPoint3.transform.localPosition.y) <= snapRange)
        {
            this.transform.localPosition = new Vector2(snapPoint3.transform.localPosition.x, snapPoint3.transform.localPosition.y);
            snapped = true;
        }
        // snapPoint4
        else if (Mathf.Abs(this.transform.localPosition.x - snapPoint4.transform.localPosition.x) <= snapRange &&
            Mathf.Abs(this.transform.localPosition.y - snapPoint4.transform.localPosition.y) <= snapRange)
        {
            this.transform.localPosition = new Vector2(snapPoint4.transform.localPosition.x, snapPoint4.transform.localPosition.y);
            snapped = true;
        }
        // snapPoint5
        else if (Mathf.Abs(this.transform.localPosition.x - snapPoint5.transform.localPosition.x) <= snapRange &&
            Mathf.Abs(this.transform.localPosition.y - snapPoint5.transform.localPosition.y) <= snapRange)
        {
            this.transform.localPosition = new Vector2(snapPoint5.transform.localPosition.x, snapPoint5.transform.localPosition.y);
            snapped = true;
        }
        // snapPoint6
        else if (Mathf.Abs(this.transform.localPosition.x - snapPoint6.transform.localPosition.x) <= snapRange &&
            Mathf.Abs(this.transform.localPosition.y - snapPoint6.transform.localPosition.y) <= snapRange)
        {
            this.transform.localPosition = new Vector2(snapPoint6.transform.localPosition.x, snapPoint6.transform.localPosition.y);
            snapped = true;
        }
        // snapPoint7
        else if (Mathf.Abs(this.transform.localPosition.x - snapPoint7.transform.localPosition.x) <= snapRange &&
            Mathf.Abs(this.transform.localPosition.y - snapPoint7.transform.localPosition.y) <= snapRange)
        {
            this.transform.localPosition = new Vector2(snapPoint7.transform.localPosition.x, snapPoint7.transform.localPosition.y);
            snapped = true;
        }
        else
        {
            Reset_Position();
        }
    }
}