using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Gacha_System_Data
{
    public Gacha_System_Item[] items;
}

[System.Serializable]
public class Gacha_System_UI
{
    public Image[] itemImages;
    public Text[] itemAmountTexts;
    public Image cropImage;
}

[System.Serializable]
public class Gacha_System_Item
{
    public Item_Info item;
    public float percentage;
    public int currentAmount;
}

public class Gacha_System : MonoBehaviour
{
    public Gacha_System_Data data;
    public Gacha_System_UI ui;

    private void Start()
    {
        Update_Item_UI();
    }

    private void Update_Item_UI()
    {
        for (int i = 0; i < data.items.Length; i++)
        {
            ui.itemImages[i].sprite = data.items[i].item.itemSprite;
            ui.itemAmountTexts[i].text = data.items[i].currentAmount.ToString();
        }
    }

    private bool Percentage_Setter(float percentage)
    {
        var value = (100 - percentage) * 0.01f;

        if (Random.value > value)
        {
            return true;
        }
        else return false;
    }
    public void Gacha_Random_Item()
    {
        bool gachaCheckComplete = false;

        while (!gachaCheckComplete)
        {
            int randomNum = Random.Range(0, data.items.Length);
            var randomItem = data.items[randomNum];

            if (Percentage_Setter(randomItem.percentage))
            {
                gachaCheckComplete = true;

                // show sprite
                ui.cropImage.sprite = randomItem.item.itemSprite;
                // +1 item amount
                randomItem.currentAmount += 1;
                // update amount ui
                Update_Item_UI();
            }
        }
    }
}
