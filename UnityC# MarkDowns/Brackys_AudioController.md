# Brackys Audio Controller

### Data
```C#
[System.Serializable]
public class AudioData
{
    private AudioSource _audioSource;
    public AudioSource audioSource => _audioSource;

    private int _audioNum;
    public int audioNum => _audioNum;

    public AudioClip clip;
    public string name;

    [Range(0f, 1f)]
    public float volume;
    
    public bool loop;

    // Constructors
    public AudioData (AudioSource setSource, int setNum)
    {
        _audioSource = setSource;
        _audioNum = setNum;
    }
}
```

### Controller
```C#
public class Audio_Controller : MonoBehaviour
{
    [SerializeField] private AudioData[] _audioClips;
    public AudioData[] audioClips => _audioClips;


    // UnityEngine
    private void Awake()
    {
        Set_AudioSource();
    }


    //
    private void Set_AudioSource()
    {
        int audioNumCount = 0;
        
        for (int i = 0; i < _audioClips.Length; i++)
        {
            AudioSource addSource = gameObject.AddComponent<AudioSource>();
            _audioClips[i] = new AudioData(addSource, audioNumCount);
            audioNumCount++;

            _audioClips[i].audioSource.clip = _audioClips[i].clip;
            _audioClips[i].audioSource.volume = _audioClips[i].volume;
            _audioClips[i].audioSource.loop = _audioClips[i].loop;
        }
    }

    public void Play_Sound(int soundNum)
    {
        for (int i = 0; i < _audioClips.Count; i++)
        {
            if (_audioClips[i].soundNum != soundNum) continue;
            _audioClips[i].audioSource.Play();
            break;
        }
    }

    public void Play_Sound(string soundName)
    {
        for (int i = 0; i < _audioClips.Count; i++)
        {
            if (_audioClips[i].name != soundName) continue;
            _audioClips[i].audioSource.Play();
            break;
        }
    }
}
```