using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCard_Track_System : MonoBehaviour
{
    public List<Card_Controller> allFieldCards = new List<Card_Controller>();

    public void Addto_Track(Card_Controller cardController)
    {
        allFieldCards.Add(cardController);
    }
    public void Removefrom_Track(Card_Controller cardController)
    {
        for (int i = 0; i < allFieldCards.Count; i++)
        {
            if (cardController != allFieldCards[i]) continue;

            allFieldCards.Remove(allFieldCards[i]);
            break;
        }
    }
}