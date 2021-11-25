using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shopController5 : MonoBehaviour
{
    [SerializeField] private Image selectedSkin5;
    [SerializeField] private Text coinsText;
    [SerializeField] private skinManager5 skinManager5;

    void Update()
    {
        coinsText.text = "Coins: " + PlayerPrefs.GetInt("Coins");
        selectedSkin5.sprite = skinManager5.GetSelectedSkin5().sprite5;
    }
}
