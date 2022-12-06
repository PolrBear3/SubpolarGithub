using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class All_Seeds_Buffs_Info_Data
{
    public bool buffInfo;
}

public class All_Seeds_Buffs_Info : MonoBehaviour
{
    public Options_Menu optionMenu;
    public All_Seeds_Buffs_Info_Data data;

    public Sprite allSeedsIconSprite;
    public Sprite allBuffsIconSprite;

    public void Toggle_Infos()
    {
        if (!data.buffInfo)
        {
            data.buffInfo = true;
            optionMenu.ui.topButtons[2].buttonIconImage.sprite = allBuffsIconSprite;
        }
        else
        {
            data.buffInfo = false;
            optionMenu.ui.topButtons[2].buttonIconImage.sprite = allSeedsIconSprite;
        }
    }
}
