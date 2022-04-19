using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enumeration_System : MonoBehaviour
{
    public enum Days { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }

    public Days currentDay;

    public void Check_Message()
    {
        switch (currentDay)
        {
            case Days.Monday:
                Debug.Log("You can fall apart");
                break;
            case Days.Tuesday:
            case Days.Wednesday:
                Debug.Log("Break my heart");
                    break;
            case Days.Thursday:
                Debug.Log("Doesn't even start");
                break;
            case Days.Friday:
                Debug.Log("I'm in love");
                break;
            case Days.Saturday:
                Debug.Log("Wait");
                break;
            case Days.Sunday:
                Debug.Log("Always comes to late");
                break;
        }
    }
}
