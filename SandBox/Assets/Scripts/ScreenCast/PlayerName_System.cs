using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerName_System : MonoBehaviour
{
    public string enteredName;

    public void Set_Name()
    {
        // has entered in a name
        if (enteredName != null && enteredName != "")
        {
            Debug.Log("Hello " + enteredName);
        }
        // is null
        else
        {
            Debug.Log("Hello player1");
        } 
    }
}
