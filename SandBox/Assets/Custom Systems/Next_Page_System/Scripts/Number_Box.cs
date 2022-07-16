using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number_Box : MonoBehaviour
{
    [HideInInspector] public int boxNumber;

    public Text boxNumberText;

    public void Update_BoxNum_Text()
    {
        boxNumberText.text = boxNumber.ToString();        
    }
}
