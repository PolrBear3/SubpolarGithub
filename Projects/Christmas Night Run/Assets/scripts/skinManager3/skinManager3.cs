using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinManager3", menuName = "Skin Manager3")]
public class skinManager3 : ScriptableObject
{
    [SerializeField] public Skin3[] skins3;
    private const string Prefix3 = "Skin_3";
    private const string SelectedSkin3 = "SelectedSkin3";

    public void SelectSkin3(int skinIndex3) => PlayerPrefs.SetInt(SelectedSkin3, skinIndex3);

    public Skin3 GetSelectedSkin3()
    {
        int skinIndex3 = PlayerPrefs.GetInt(SelectedSkin3, 0);
        if (skinIndex3 >= 0 && skinIndex3 < skins3.Length)
        {
            return skins3[skinIndex3];
        }
        else
        {
            return null;
        }
    }
    public void Unlock3(int skinIndex3) => PlayerPrefs.SetInt(Prefix3 + skinIndex3, 1);

    public bool IsUnlocked3(int skinIndex3) => PlayerPrefs.GetInt(Prefix3 + skinIndex3, 0) == 1;
}
