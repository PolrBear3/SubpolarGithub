using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_System : MonoBehaviour
{
    public GameObject[] objects = new GameObject[0];
    public int bagLevel;

    public void Turn_On_and_Off()
    {
        switch (bagLevel)
        {
            case 1:
                if (!objects[0].activeSelf)
                {
                    objects[0].SetActive(true);
                }
                else
                {
                    objects[0].SetActive(false);
                }
                break;
            case 2:
                if (!objects[1].activeSelf)
                {
                    objects[1].SetActive(true);
                }
                else
                {
                    objects[1].SetActive(false);
                }
                break;
            case 3:
                if (!objects[2].activeSelf)
                {
                    objects[2].SetActive(true);
                }
                else
                {
                    objects[2].SetActive(false);
                }
                break;
            default:
                Debug.Log("wrong number");
                break;
        }
    }
}
