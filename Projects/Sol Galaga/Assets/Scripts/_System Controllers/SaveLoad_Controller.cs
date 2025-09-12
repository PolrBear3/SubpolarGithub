using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public interface ISaveLoadable
{
    void Save_Data();
    void Load_Data();
}
 
public class SaveLoad_Controller : MonoBehaviour
{
    public static SaveLoad_Controller instance;
    
    
    // UnityEngine
    private void Awake()
    {
        instance = this;
        
        LoadAll_ISaveLoadable();
    }

    private void OnApplicationQuit()
    {
        SaveAll_ISaveLoadable();
    }


    // Main
    private static List<ISaveLoadable> All_ISaveLoadables()
    {
        IEnumerable<ISaveLoadable> saveLoadableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveLoadable>();
        return new(saveLoadableObjects);
    }

    public void SaveAll_ISaveLoadable()
    {
        for (int i = 0; i < All_ISaveLoadables().Count; i++)
        {
            All_ISaveLoadables()[i].Save_Data();
        }
    }
    
    private void LoadAll_ISaveLoadable()
    {
        for (int i = 0; i < All_ISaveLoadables().Count; i++)
        {
            All_ISaveLoadables()[i].Load_Data();
        }
    }
}