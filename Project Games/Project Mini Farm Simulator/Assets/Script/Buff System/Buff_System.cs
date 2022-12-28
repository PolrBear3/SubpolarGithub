using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{
    void Activate_Buff();
}
public interface IBuffResetable
{
    void Reset_Buff();
}

public class Buff_System : MonoBehaviour
{
    public MainGame_Controller controller;

    public GameObject buffs;
    private List<GameObject> allBuffs = new List<GameObject>();

    private void Start()
    {
        Set_All_Buffs();
    }

    private void Set_All_Buffs()
    {
        for (int i = 0; i < buffs.transform.childCount; i++)
        {
            allBuffs.Add(buffs.transform.GetChild(i).gameObject);
        }
    }
    public void Activate_All_Buffs()
    {
        for (int i = 0; i < allBuffs.Count; i++)
        {
            if (allBuffs[i].TryGetComponent(out IBuff b))
            {
                b.Activate_Buff();
            }
        }
    }
    public void Reset_All_Buffs()
    {
        for (int i = 0; i < allBuffs.Count; i++)
        {
            if (allBuffs[i].TryGetComponent(out IBuffResetable b))
            {
                b.Reset_Buff();
            }
        }
    }
}
