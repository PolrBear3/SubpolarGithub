using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound_Controller : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField] private List<Sound_Clip> _soundClips = new List<Sound_Clip>();
    public List<Sound_Clip> soundClips { get => _soundClips; set => _soundClips = value; }

    //
    private void Awake()
    {
        if (!gameObject.TryGetComponent(out AudioSource audioSource)) return;

        _audioSource = audioSource;
        Set_AudioSource();
    }

    //
    private void Set_AudioSource()
    {
        int numCount = 0;
        for (int i = 0; i < soundClips.Count; i++)
        {
            soundClips[i].soundNum = numCount;
            soundClips[i].audioSource = gameObject.AddComponent<AudioSource>();
            soundClips[i].audioSource.clip = soundClips[i].clip;
            soundClips[i].audioSource.volume = soundClips[i].volume;
            soundClips[i].audioSource.loop = soundClips[i].loop;
            numCount++;
        }
    }

    public void Play_Sound(int soundNum)
    {
        for (int i = 0; i < soundClips.Count; i++)
        {
            if (soundClips[i].soundNum != soundNum) continue;
            soundClips[i].audioSource.Play();
            break;
        }
    }

    public void Play_Sound(string soundName)
    {
        for (int i = 0; i < soundClips.Count; i++)
        {
            if (soundClips[i].name != soundName) continue;
            soundClips[i].audioSource.Play();
            break;
        }
    }
}
