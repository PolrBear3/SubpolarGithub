using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound_Clip
{
    [HideInInspector] public AudioSource audioSource;

    public AudioClip clip;
    [HideInInspector] public int soundNum;

    public string name;

    [Range(0f, 1f)]
    public float volume;

    public bool loop;
}
