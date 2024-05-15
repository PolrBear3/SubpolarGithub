using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
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

        foreach (var audio in _audioClips)
        {
            audio.audioSource = gameObject.AddComponent<AudioSource>();

            audio.audioNum = audioNumCount;
            audioNumCount++;

            audio.audioSource.clip = audio.clip;
            audio.audioSource.volume = audio.volume;
            audio.audioSource.loop = audio.loop;
        }
    }

    public void Play_Sound(int soundNum)
    {
        for (int i = 0; i < _audioClips.Length; i++)
        {
            if (_audioClips[i].audioNum != soundNum) continue;
            _audioClips[i].audioSource.Play();
            break;
        }
    }

    public void Play_Sound(string soundName)
    {
        for (int i = 0; i < _audioClips.Length; i++)
        {
            if (_audioClips[i].name != soundName) continue;
            _audioClips[i].audioSource.Play();
            Debug.Log("check");
            break;
        }
    }
}
