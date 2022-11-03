using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Position_Controller_Data
{

}

[System.Serializable]
public class Position_Controller_UI
{

}

public class Position_Controller : MonoBehaviour
{
    public Controller mainController;
    public Position_Controller_Data data;
    public Position_Controller_UI ui;

    public Spell_Button buttonD;
    public Spell_Button buttonF;
}
