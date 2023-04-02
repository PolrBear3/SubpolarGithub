using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound_Controller : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource sfxSource2;
    [SerializeField] private AudioSource sfxSource3;

    public void Play_BGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }
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

    public void BGM_Volume_Control(float value)
    {
        bgmSource.volume = value;
    }
    public void SFX_Volume_Control(float value)
    {
        sfxSource.volume = value;
        sfxSource2.volume = value;
        sfxSource3.volume = value;
    }
}
