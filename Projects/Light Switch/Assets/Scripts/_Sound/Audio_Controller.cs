using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[System.Serializable]
public class SoundData
{
    [SerializeField] private EventReference _reference;
    public EventReference reference => _reference;

    private EventInstance _instance;
    public EventInstance instance => _instance;


    // Constructors
    public EventInstance Set_Instance(EventInstance setInstance)
    {
        _instance = setInstance;
        return _instance;
    }
}

public class Audio_Controller : MonoBehaviour
{
    private List<SoundData> _instanceDatas = new();
    public List<SoundData> instanceDatas => _instanceDatas;

    private List<StudioEventEmitter> _emitters = new();
    public List<StudioEventEmitter> emitters => _emitters;

    
    // References
    private EventReference Play_OneShot(EventReference soundReference)
    {
        if (soundReference.IsNull) return soundReference;

        RuntimeManager.PlayOneShot(soundReference, transform.position);
        return soundReference;
    }
    public void Play_OneShot(GameObject prefab, int dataIndexNum)
    {
        if (prefab == null) return;
        
        SoundData_Controller dataController = prefab.GetComponent<SoundData_Controller>();
        if (dataController == null) return;

        if (dataController.soundDatas == null || dataController.soundDatas.Length <= 0) return;
        if (dataIndexNum < 0 || dataIndexNum >= dataController.soundDatas.Length) return;
        
        EventReference reference = dataController.soundDatas[dataIndexNum].reference;
        Play_OneShot(reference);
    }
    
    public void Set_EventInstance_Parameter(SoundData targetData, string parameterName, float setValue)
    {
        targetData.instance.setParameterByName(parameterName, setValue);
    }


    // Event Instances
    public SoundData EventInstance_Data(SoundData data)
    {
        for (int i = 0; i < _instanceDatas.Count; i++)
        {
            if (data != _instanceDatas[i]) continue;
            return _instanceDatas[i];
        }

        return null;
    }

    public EventInstance EventInstance(SoundData data)
    {
        return EventInstance_Data(data).instance;
    }
    public EventInstance EventInstance(GameObject prefab, int dataIndexNum)
    {
        SoundData eventInstance = prefab.GetComponent<SoundData_Controller>().soundDatas[dataIndexNum];
        return EventInstance(eventInstance);
    }


    private EventInstance Create_EventInstance(SoundData data)
    {
        for (int i = 0; i < _instanceDatas.Count; i++)
        {
            if (_instanceDatas[i] == data) return data.instance;
        }

        EventInstance instanceToAdd = RuntimeManager.CreateInstance(data.reference);
        data.Set_Instance(instanceToAdd);

        _instanceDatas.Add(data);

        return instanceToAdd;
    }
    public EventInstance Create_EventInstance(GameObject prefab, int dataIndexNum)
    {
        SoundData eventInstance = prefab.GetComponent<SoundData_Controller>().soundDatas[dataIndexNum];
        
        if (instanceDatas.Contains(eventInstance)) return EventInstance(eventInstance);
        
        Create_EventInstance(eventInstance);
        return eventInstance.instance;
    }

    private void Remove_EventInstance(SoundData data)
    {
        _instanceDatas.Remove(data);

        data.instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        data.instance.release();
    }
    public void Remove_EventInstance(GameObject prefab, int dataIndexNum)
    {
        SoundData eventInstance = prefab.GetComponent<SoundData_Controller>().soundDatas[dataIndexNum];
        Remove_EventInstance(eventInstance);
    }


    private void Clear_EventInstances()
    {
        for (int i = _instanceDatas.Count - 1; i >= 0; i--)
        {
            Remove_EventInstance(_instanceDatas[i]);
        }
    }
    public void Clear_EventInstances(SoundData[] clearDatas)
    {
        foreach (SoundData data in clearDatas)
        {
            Remove_EventInstance(data);
        }
    }
    

    // Event Emitters
    public StudioEventEmitter EventEmitter(GameObject currentPrefab)
    {
        StudioEventEmitter currentEmitter = currentPrefab.GetComponent<StudioEventEmitter>();

        for (int i = 0; i < _emitters.Count; i++)
        {
            if (currentEmitter != _emitters[i]) continue;
            return _emitters[i];
        }
        return null;
    }


    public void Track_EventEmitter(GameObject trackPrefab)
    {
        _emitters.Add(trackPrefab.GetComponent<StudioEventEmitter>());
    }

    public void UnTrack_EventEmitter(GameObject trackPrefab)
    {
        StudioEventEmitter untrackEmitter = EventEmitter(trackPrefab);

        untrackEmitter.Stop();
        _emitters.Remove(untrackEmitter);
    }
}
