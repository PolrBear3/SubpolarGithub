using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable_Button : MonoBehaviour
{
    public CollectableRoom_Menu menu;
    // public collectable ScrObj
    // public collectable image connection
    // public text collectable current amount connection

    public void Select_Collectable()
    {
        menu.AllFrame_PlaceMode_On();
        // keep this button pressed
        // reset menu currently selected collectable
        // menu current selected collectable to this ScrObj
    }
}
