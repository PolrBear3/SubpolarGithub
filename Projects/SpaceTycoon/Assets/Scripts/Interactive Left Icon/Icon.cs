using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{
    public GameObject iconBoxCollider;

    public void Set_Icon_to_Default_Position()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }

    public void Set_Icon_Position()
    {
        if (Icon_Points_Manager.point1Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 120);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 120);
        }
        else if (Icon_Points_Manager.point2Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 60);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 60);
        }
        else if (Icon_Points_Manager.point3Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 0);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 0);
        }
        else if (Icon_Points_Manager.point4Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -60);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -60);
        }
        else if (Icon_Points_Manager.point5Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -120);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -120);
        }
    }
}
