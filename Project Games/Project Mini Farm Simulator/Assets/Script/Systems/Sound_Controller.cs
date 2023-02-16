using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound_Controller : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public void Play_SFX(AudioClip clip)
    {
        sfxSource.clip = clip;
        sfxSource.Play();
    }
}
