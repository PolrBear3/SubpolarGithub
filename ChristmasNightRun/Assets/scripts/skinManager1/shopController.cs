using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shopController : MonoBehaviour
{
    [SerializeField] private Image selectedSkin;
    [SerializeField] private Text coinsText;
    [SerializeField] private skinManager skinManager;

    // Update is called once per frame
    void Update()
    {
        coinsText.text = "" + PlayerPrefs.GetInt("Coins");
        selectedSkin.sprite = skinManager.GetSelectedSkin().sprite;
    }
}
