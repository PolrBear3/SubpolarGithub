using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class other2 : MonoBehaviour
{
    [SerializeField] private secondSkinManager skinManager2;
    void Start()
    {
        gameObject.GetComponent<Image>().sprite = skinManager2.GetSelectedSkin2().sprite2;
    }
}
