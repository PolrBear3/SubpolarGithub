using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface ISaveLoadable
{
    void Save_Data();
    void Load_Data();
}
 
public class SaveLoad_Controller : MonoBehaviour
{
    // UnityEngine
    private void Awake()
    {
        for (int i = 0; i < All_ISaveLoadables().Count; i++)
        {
            All_ISaveLoadables()[i].Load_Data();
        }
    }

    private void OnApplicationQuit()
    {
        SaveAll_All_ISaveLoadable();
    }



    //
    public static void SaveAll_All_ISaveLoadable()
    {
        for (int i = 0; i < All_ISaveLoadables().Count; i++)
        {
            All_ISaveLoadables()[i].Save_Data();
        }

        ES3.CreateBackup("TastyRoad_backup");
    }

    private static List<ISaveLoadable> All_ISaveLoadables()
    {
        IEnumerable<ISaveLoadable> saveLoadableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveLoadable>();
        return new (saveLoadableObjects);
    }
}