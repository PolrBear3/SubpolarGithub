using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    public DataBase dataBase;

    public FieldCard_Track_System trackSystem;

    private void Start()
    {
        Spawn_Card(0, 3);
    }

    // ingame system
    public void Spawn_Card(int id, int amount)
    {
        var info = dataBase.Find_Food(id);

        for (int i = 0; i < amount; i++)
        {
            float randomPos = Random.Range(-7, 7);

            var spawnCard = Instantiate(dataBase.card, new Vector2(randomPos, 0), Quaternion.identity).GetComponent<Card_Controller>();
            spawnCard.New_Card(this, dataBase.Find_Food(0), null);

            trackSystem.Addto_Track(spawnCard);
        }
    }
}
