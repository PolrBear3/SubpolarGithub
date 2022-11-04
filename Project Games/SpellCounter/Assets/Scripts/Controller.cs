using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Position_Controller[] positions;

    private void Start()
    {
        Clear_All_SpellButtons();
    }

    private void Clear_All_SpellButtons()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i].buttonD.Clear_Spell();
            positions[i].buttonF.Clear_Spell();
        }
    }

    public void UnSelect_All_SpellButtons()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i].buttonD.UnSelect();
            positions[i].buttonF.UnSelect();
        }
    }
}
