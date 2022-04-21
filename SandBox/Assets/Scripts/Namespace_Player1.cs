using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct player1_Stats
{
    public string name;
    public int health;
    public int power;
}

public class Namespace_Player1 : MonoBehaviour
{
    player1_Stats player1stats = new player1_Stats();
    Player2.player2_Stats player2stats = new Player2.player2_Stats();

    void Set_Stats()
    {
        player1stats.name = "Mario";
        player1stats.health = 10;
        player1stats.power = 5;

        player2stats.name = "Wario";
        player2stats.health = 13;
        player2stats.power = 4;
    }
}
