using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{
    public GameObject iconBoxCollider;
    
    private void Start()
    {
        Set_Icon_Position();
    }

    void Set_Icon_Position()
    {
        if (Icon_Points_Manager.point1Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 300);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 300);
        }
        else if (Icon_Points_Manager.point2Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 240);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 240);
        }
        else if (Icon_Points_Manager.point3Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 180);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 180);
        }
        else if (Icon_Points_Manager.point4Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 120);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 120);
        }
        else if (Icon_Points_Manager.point5Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 60);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 60);
        }
        else if (Icon_Points_Manager.point6Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 0);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, 0);
        }
        else if (Icon_Points_Manager.point7Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -60);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -60);
        }
        else if (Icon_Points_Manager.point8Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -120);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -120);
        }
        else if (Icon_Points_Manager.point9Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -180);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -180);
        }
        else if (Icon_Points_Manager.point10Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -240);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -240);
        }
        else if (Icon_Points_Manager.point11Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -300);
            iconBoxCollider.GetComponent<RectTransform>().anchoredPosition = new Vector2(-576, -300);
        }
    }
}
