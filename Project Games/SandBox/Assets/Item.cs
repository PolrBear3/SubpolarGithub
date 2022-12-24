using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Item_Data
{
    public int health;
    public int water;
}

[System.Serializable]
public struct Item_UI
{
    public Text healthText;
    public Text waterText;
}

public class Item : MonoBehaviour, IBuffable
{
    public Item_Data data;
    public Item_UI ui;

    private void Start()
    {
        UI_Update();
    }

    public void Give_Buff()
    {
        Add_Health(2);
        UI_Update();
    }

    private void UI_Update()
    {
        ui.healthText.text = data.health.ToString();
        ui.waterText.text = data.water.ToString();
    }

    private void Add_Health(int amount)
    {
        data.health += amount;
    }
}
