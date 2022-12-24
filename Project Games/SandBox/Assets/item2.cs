using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Item2_Data
{
    public int health;
    public int water;
}

[System.Serializable]
public struct Item2_UI
{
    public Text healthText;
    public Text waterText;
}

public class item2 : MonoBehaviour, IBuffable
{
    public Item2_Data data;
    public Item2_UI ui;

    private void Start()
    {
        UI_Update();
    }

    public void Give_Buff()
    {
        Add_Water(2);
        UI_Update();
    }

    private void UI_Update()
    {
        ui.healthText.text = data.health.ToString();
        ui.waterText.text = data.water.ToString();
    }

    private void Add_Water(int amount)
    {
        data.water += amount;
    }
}
