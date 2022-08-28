using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinManager2", menuName = "Skin Manager2")]
public class secondSkinManager : ScriptableObject
{
    [SerializeField] public Skin2[] skins2;
    private const string Prefix2 = "Skin_2";
    private const string SelectedSkin2 = "SelectedSkin2";

    public void SelectSkin2(int skinIndex2) => PlayerPrefs.SetInt(SelectedSkin2, skinIndex2);

    public Skin2 GetSelectedSkin2()
    {
        int skinIndex2 = PlayerPrefs.GetInt(SelectedSkin2, 0);
        if (skinIndex2 >= 0 && skinIndex2 < skins2.Length)
        {
            return skins2[skinIndex2];
        }
        else
        {
            return null;
        }
    }
    public void Unlock2(int skinIndex2) => PlayerPrefs.SetInt(Prefix2 + skinIndex2, 1);

    public bool IsUnlocked2(int skinIndex2) => PlayerPrefs.GetInt(Prefix2 + skinIndex2, 0) == 1;
}
