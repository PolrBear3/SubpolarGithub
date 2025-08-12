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

public interface IBackupLoadable
{
    bool Has_Conflict();
    void Load_Backup();
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
        
        for (int i = 0; i < saveLoadables.Count; i++)
        {
            saveLoadables[i].Save_Data();
        }
    }
    
    private void LoadAll_ISaveLoadable()
    {
        List<ISaveLoadable> saveLoadables = All_ISaveLoadables();
        
        for (int i = 0; i < saveLoadables.Count; i++)
        {
            var saveObject = saveLoadables[i];
            saveObject.Load_Data();
            
            if (saveObject is not IBackupLoadable backupLoadable) continue;
            if (backupLoadable.Has_Conflict() == false) continue;
            
            backupLoadable.Load_Backup();
        }
    }


    public void Load_IBackupLoadable(IBackupLoadable backupLoadable)
    {
        if (backupLoadable.Has_Conflict() == false) return;
        backupLoadable.Load_Backup();
    }
}