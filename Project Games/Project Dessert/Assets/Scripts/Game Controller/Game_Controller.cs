using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    public DataBase dataBase;

    public FieldCard_Track_System trackSystem;

    /* public void Spawn_Card(int id, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float randomPos = Random.Range(-7, 7);

            var spawnCard = Instantiate(dataBase.blankCard, new Vector2(randomPos, 0), Quaternion.identity).GetComponent<Card_Controller>();
            spawnCard.Update_Card(this, dataBase.Find_Food(id), null);

            trackSystem.Addto_Track(spawnCard);
        }
    } */
}
