using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogData
{
    public Sprite icon;

    [TextArea(2, 2)]
    public string info;


    //
    public DialogData (Sprite icon, string info)
    {
        this.icon = icon;
        this.info = info;
    }
}
