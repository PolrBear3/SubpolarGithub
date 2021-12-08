using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPShop : MonoBehaviour
{
    private string Joys5000 = "com.SubpolarGames.ChristmasNightRun.5000Joys";
    public void OnPurchaseComplete5000Joys(Product product)
    {
        if (product.definition.id == Joys5000)
        {
            coinText.AddCoins5000();
            Debug.Log("5000Joys purchased");
        }
    }

    private string Joys10000 = "com.SubpolarGames.ChristmasNightRun.10000Joys";
    public void OnPurchaseComplete10000Joys(Product product)
    {
        if (product.definition.id == Joys10000)
        {
            coinText.AddCoins10000();
            Debug.Log("10000Joys purchased");
        }
    }

    private string Joys30000 = "com.SubpolarGames.ChristmasNightRun.30000Joys";
    public void OnPurchaseComplete30000Joys(Product product)
    {
        if (product.definition.id == Joys30000)
        {
            coinText.AddCoins30000();
            Debug.Log("30000Joys purchased");
        }
    }

    private string Joys50000 = "com.SubpolarGames.ChristmasNightRun.50000Joys";
    public void OnPurchaseComplete50000Joys(Product product)
    {
        if (product.definition.id == Joys50000)
        {
            coinText.AddCoins50000();
            Debug.Log("50000Joys purchased");
        }
    }

    private string Joys100000 = "com.SubpolarGames.ChristmasNightRun.100000Joys";
    public void OnPurchaseComplete100000Joys(Product product)
    {
        if (product.definition.id == Joys100000)
        {
            coinText.AddCoins100000();
            Debug.Log("100000Joys purchased");
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase failed");
    }
}
