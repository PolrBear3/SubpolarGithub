using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinManager4", menuName = "Skin Manager4")]
public class skinManager4 : ScriptableObject
{
    [SerializeField] public Skin4[] skins4;
    private const string Prefix4 = "Skin_4";
    private const string SelectedSkin4 = "SelectedSkin4";

    public void SelectSkin4(int skinIndex4) => PlayerPrefs.SetInt(SelectedSkin4, skinIndex4);

    public Skin4 GetSelectedSkin4()
    {
        int skinIndex4 = PlayerPrefs.GetInt(SelectedSkin4, 0);
        if (skinIndex4 >= 0 && skinIndex4 < skins4.Length)
        {
            return skins4[skinIndex4];
        }
        else
        {
            return null;
        }
    }
    public void Unlock4(int skinIndex4) => PlayerPrefs.SetInt(Prefix4 + skinIndex4, 1);

    public bool IsUnlocked4(int skinIndex4) => PlayerPrefs.GetInt(Prefix4 + skinIndex4, 0) == 1;
}
