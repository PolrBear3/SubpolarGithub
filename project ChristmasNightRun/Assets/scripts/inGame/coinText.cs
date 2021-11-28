using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class coinText : MonoBehaviour
{
    [SerializeField] private Text coinsText;
    private static int coins;

    void Start()
    {
        coins = PlayerPrefs.GetInt("Coins");
    }

    void Update()
    {
        coinsText.text = "" + coins;
    }

    public static void AddCoins()
    {
        coins += 1;
    }

    public static void coinSave()
    {
        PlayerPrefs.SetInt("Coins", coins);
    }
}
