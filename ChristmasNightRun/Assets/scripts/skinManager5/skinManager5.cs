using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinManager5", menuName = "Skin Manager5")]
public class skinManager5 : ScriptableObject
{
    [SerializeField] public Skin5[] skins5;
    private const string Prefix5 = "Skin_5";
    private const string SelectedSkin5 = "SelectedSkin5";

    public void SelectSkin5(int skinIndex5) => PlayerPrefs.SetInt(SelectedSkin5, skinIndex5);

    public Skin5 GetSelectedSkin5()
    {
        int skinIndex5 = PlayerPrefs.GetInt(SelectedSkin5, 0);
        if (skinIndex5 >= 0 && skinIndex5 < skins5.Length)
        {
            return skins5[skinIndex5];
        }
        else
        {
            return null;
        }
    }
    public void Unlock5(int skinIndex5) => PlayerPrefs.SetInt(Prefix5 + skinIndex5, 1);

    public bool IsUnlocked5(int skinIndex5) => PlayerPrefs.GetInt(Prefix5 + skinIndex5, 0) == 1;
}
