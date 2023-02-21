using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound_Controller : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource sfxSource2;
    [SerializeField] private AudioSource sfxSource3;

    public void Play_SFX(AudioClip clip)
    {
        if (!sfxSource.isPlaying)
        {
            sfxSource.clip = clip;
            sfxSource.Play();
        }
        else if (!sfxSource2.isPlaying)
        {
            sfxSource2.clip = clip;
            sfxSource2.Play();
        }
        else
        {
            sfxSource3.clip = clip;
            sfxSource3.Play();
        }
    }
}
