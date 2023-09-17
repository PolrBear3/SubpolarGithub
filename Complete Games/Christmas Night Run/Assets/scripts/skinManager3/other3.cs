using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class other3 : MonoBehaviour
{
    [SerializeField] private skinManager3 skinManager3;
    void Start()
    {
        gameObject.GetComponent<Image>().sprite = skinManager3.GetSelectedSkin3().sprite3;
    }
}
