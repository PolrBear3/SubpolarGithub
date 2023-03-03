using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    public DataBase dataBase;

    private void Start()
    {
        Spawn_Card(0, 3);
    }

    public void Spawn_Card(int id, int amount)
    {
        var info = dataBase.Find_Food(id);

        for (int i = 0; i < amount; i++)
        {
            var spawnCard = Instantiate(dataBase.card, Vector2.zero, Quaternion.identity).GetComponent<Card_Controller>();
            spawnCard.Update_Card(info.foodSprite, dataBase.Find_CardType_Icon(info.type));
        }
    }
}
