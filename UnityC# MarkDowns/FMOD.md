# FMOD
```C#
using FMODUnity
using FMOD.Studio
```

### One Shot (Event Defaults ⇒ 2D Action)
```C#
[SerializeField] private EventReference _soundExample
    
public void Play_OneShot()
{
    RuntimeManager.PlayOneShot(_soundExample, transform.position);
}
```

### Event Instance & Loop (Event Defaults ⇒ 2D Timeline)
```C#
[SerializeField] private EventReference _soundExample

private EventInstance _eventInstance;
		
private void Create_EventInstance()
{
	_eventInstance = RuntimeManager.CreateInstance(_soundExample);
}
		
public void Play_EventInstance()
{
	_eventInstance.start();
}
		
public void Stop_EventInstance()
{
	_eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
}
		
public void Destroy_EventInstance()
{
	Stop_EventInstance();
	_eventInstance.release();
}
```

### Event Emitter & Audio by distance
- Add Effect to timeline ⇒ FMOD Spatializer
- add componenet to gameobject ⇒ FMOD Studio Event Emitter
```C#
private StudioEventEmitter _eventEmitter;
		
private void Awake()
{
	_eventEmitter = gameObject.GetComponent<StudioEventEmitter>();
}
		
public void Play_EventEmitter()
{
	_eventInstance.Play();
}
		
public void Stop_EventEmitter()
{
	_eventInstance.Stop();
}
```

### Adding and using Parameter
- window ⇒ preset browser ⇒ new parameter
- add effect ⇒ add automation
```C#
[SerializeField] private EventReference _soundExample
private EventInstance _eventInstance;
		
private void Create_EventInstance()
{
	_eventInstance = RuntimeManager.CreateInstance(_soundExample);
}
		
private void Set_EventInstance_Parameter(string parameterName, float setValue)
{
	_eventInstance.setParameterByName(parameterName, setValue);
}
```

### Mixer Bus Control (volume control by group)
- window ⇒ mixer
- add new group (A, B)
```C#
private Bus masterBus;
private Bus aBus;
private Bus bBus;
		
private void Awake()
{
	masterBus = RuntimeManager.GetBus("bus:/");
	aBus = RuntimeManager.GetBus("bus:/A");
	bBus = RuntimeManager.GetBus("bus:/B");
}
		
private void Set_MasterBus_Volume(float setValue)
{
	masterBus.setVolume(setValue);
}
```