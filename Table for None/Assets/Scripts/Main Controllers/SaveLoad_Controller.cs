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


    // Gets
    private static List<ISaveLoadable> All_ISaveLoadables()
    {
        IEnumerable<ISaveLoadable> saveLoadableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveLoadable>();
        return new(saveLoadableObjects);
    }

    
    // Main
    public void SaveAll_ISaveLoadable()
    {
        List<ISaveLoadable> saveLoadables = All_ISaveLoadables();

        foreach (ISaveLoadable data in saveLoadables)
        {
            data.Save_Data();
        }
    }
    
    private void LoadAll_ISaveLoadable()
    {
        List<ISaveLoadable> saveLoadables = All_ISaveLoadables();
        
        foreach (ISaveLoadable data in saveLoadables)
        {
            data.Load_Data();
        }
    }
}