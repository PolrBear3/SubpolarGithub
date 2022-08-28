using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class skinShopItem3 : MonoBehaviour
{
    [SerializeField] private skinManager3 skinManager3;
    [SerializeField] private int skinIndex3;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text costText;
    private Skin3 skin3;

    void Start()
    {
        skin3 = skinManager3.skins3[skinIndex3];

        GetComponent<Image>().sprite = skin3.sprite3;

        if (skinManager3.IsUnlocked3(skinIndex3))
        {
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            costText.text = skin3.cost3.ToString();
        }
    }

    public void OnSkinPressed3()
    {
        if (skinManager3.IsUnlocked3(skinIndex3))
        {
            skinManager3.SelectSkin3(skinIndex3);
        }
    }

    public void OnBuyButtonPressed3()
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);

        // Unlock the skin
        if (coins >= skin3.cost3 && !skinManager3.IsUnlocked3(skinIndex3))
        {
            PlayerPrefs.SetInt("Coins", coins - skin3.cost3);
            skinManager3.Unlock3(skinIndex3);
            buyButton.gameObject.SetActive(false);
            skinManager3.SelectSkin3(skinIndex3);
        }
        else
        {
            Debug.Log("Not enough coins :(");
        }
    }
}
