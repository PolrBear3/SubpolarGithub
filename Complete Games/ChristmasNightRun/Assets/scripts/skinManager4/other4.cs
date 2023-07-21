using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class other4 : MonoBehaviour
{
    [SerializeField] private skinManager4 skinManager4;
    void Start()
    {
        gameObject.GetComponent<Image>().sprite = skinManager4.GetSelectedSkin4().sprite4;
    }
}
