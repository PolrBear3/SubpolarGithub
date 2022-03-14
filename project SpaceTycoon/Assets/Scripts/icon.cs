using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{
    private void Start()
    {
        Set_Icon_Position();
    }

    void Set_Icon_Position()
    {
        if (SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod == 0)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, 300);
            SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod += 1;
        }
        else if (SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod == 1 && SpaceTycoon_Main_GameController.point2Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, 240);
            SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod += 1;
        }
        else if (SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod == 2 && SpaceTycoon_Main_GameController.point3Empty == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, 180);
            SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod += 1;
        }
        else if (SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod == 3)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, 120);
            SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod += 1;
        }
        else if (SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod == 4)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, 60);
            SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod += 1;
        }
        else if (SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod == 5)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, 0);
            SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod += 1;
        }
        else if (SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod == 6)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, -120);
            SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod += 1;
        }
        else if (SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod == 7)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, -180);
            SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod += 1;
        }
        else if (SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod == 8)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, -240);
            SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod += 1;
        }
        else if (SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod == 9)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, -300);
            SpaceTycoon_Main_GameController.currentIconNumbers_EscapePod += 1;
        }
    }
}
