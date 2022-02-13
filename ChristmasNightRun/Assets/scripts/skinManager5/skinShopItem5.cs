using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skinShopItem5 : MonoBehaviour
{
    [SerializeField] private skinManager5 skinManager5;
    [SerializeField] private int skinIndex5;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text costText;
    private Skin5 skin5;

    void Start()
    {
        skin5 = skinManager5.skins5[skinIndex5];

        GetComponent<Image>().sprite = skin5.sprite5;

        if (skinManager5.IsUnlocked5(skinIndex5))
        {
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            costText.text = skin5.cost5.ToString();
        }
    }

    public void OnSkinPressed5()
    {
        if (skinManager5.IsUnlocked5(skinIndex5))
        {
            skinManager5.SelectSkin5(skinIndex5);
        }
    }

    public void OnBuyButtonPressed5()
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);

        // Unlock the skin
        if (coins >= skin5.cost5 && !skinManager5.IsUnlocked5(skinIndex5))
        {
            PlayerPrefs.SetInt("Coins", coins - skin5.cost5);
            skinManager5.Unlock5(skinIndex5);
            buyButton.gameObject.SetActive(false);
            skinManager5.SelectSkin5(skinIndex5);
        }
        else
        {
            Debug.Log("Not enough coins :(");
        }
    }
}
