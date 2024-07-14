using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[System.Serializable]
public class SoundData
{
    [SerializeField] private string _eventName;
    public string eventName => _eventName;

    [SerializeField] private EventReference _eventReference;
    public EventReference eventReference => _eventReference;

    private GameObject _eventPrefab;
    public GameObject eventPrefab => _eventPrefab;

    private EventInstance _eventInstance;
    public EventInstance eventInstance => _eventInstance;


    // Constructors
    public SoundData(string setName, GameObject setPrefab, EventInstance setInstance)
    {
        _eventName = setName;
        _eventPrefab = setPrefab;
        _eventInstance = setInstance;
    }
}

public class Audio_Controller : MonoBehaviour
{
    public static Audio_Controller instance;

    [Header("")]
    [SerializeField] private SoundData[] _soundDatas;

    private List<SoundData> _eventInstances = new();
    public List<SoundData> eventInstances => _eventInstances;

    private List<StudioEventEmitter> _eventEmitters = new();
    public List<StudioEventEmitter> eventEmitters => _eventEmitters;


    // UnityEngine
    private void Awake()
    {
        instance = this;
    }


    // Gets
    private SoundData EventInstance_Data(GameObject currentPrefab)
    {
        for (int i = 0; i < _eventInstances.Count; i++)
        {
            if (currentPrefab != _eventInstances[i].eventPrefab) continue;
            return _eventInstances[i];
        }

        Debug.Log("Event Instance not found in current list");
        return null;
    }

    public EventInstance EventInstance(GameObject currentPrefab)
    {
        return EventInstance_Data(currentPrefab).eventInstance;
    }

    public StudioEventEmitter EventEmitter(GameObject currentPrefab)
    {
        StudioEventEmitter currentEmitter = currentPrefab.GetComponent<StudioEventEmitter>();

        for (int i = 0; i < _eventEmitters.Count; i++)
        {
            if (currentEmitter != _eventEmitters[i]) continue;
            return _eventEmitters[i];
        }
        return null;
    }


    // One Shot
    public void Play_OneShot(EventReference sound, Vector2 position)
    {
        RuntimeManager.PlayOneShot(sound, position);
    }
    public void Play_OneShot(string soundName, Vector2 position)
    {
        for (int i = 0; i < _soundDatas.Length; i++)
        {
            if (soundName != _soundDatas[i].eventName) continue;

            Play_OneShot(_soundDatas[i].eventReference, position);
            return;
        }
    }


    // Event Instance
    public void Create_EventInstance(string soundName, GameObject setPrefab)
    {
        for (int i = 0; i < _soundDatas.Length; i++)
        {
            if (soundName != _soundDatas[i].eventName) continue;

            EventInstance eventInstance = RuntimeManager.CreateInstance(_soundDatas[i].eventReference);
            _eventInstances.Add(new(soundName, setPrefab, eventInstance));

            return;
        }
    }

    public void Remove_EventInstance(GameObject setPrefab)
    {
        SoundData removeInstance = EventInstance_Data(setPrefab);

        removeInstance.eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        removeInstance.eventInstance.release();

        _eventInstances.Remove(removeInstance);
    }

    public void ClearAll_EventInstances()
    {

    }


    public void Set_EventInstance_Parameter(GameObject currentPrefab, string parameterName, float setValue)
    {
        EventInstance(currentPrefab).setParameterByName(parameterName, setValue);
    }
    public void Set_EventInstance_Parameter(GameObject currentPrefab, string parameterName, string label)
    {
        EventInstance(currentPrefab).setParameterByNameWithLabel(parameterName, label);
    }


    // Event Emitter
    public void Track_EventEmitter(GameObject trackPrefab)
    {
        _eventEmitters.Add(trackPrefab.GetComponent<StudioEventEmitter>());
    }

    public void UnTrack_EventEmitter(GameObject trackPrefab)
    {
        StudioEventEmitter untrackEmitter = EventEmitter(trackPrefab);

        untrackEmitter.Stop();
        _eventEmitters.Remove(untrackEmitter);
    }
}
