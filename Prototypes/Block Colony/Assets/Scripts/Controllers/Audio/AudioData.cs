using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioData
{
    public AudioSource audioSource;

    public int audioNum;

    public AudioClip clip;
    public string name;

    [Range(0f, 1f)]
    public float volume;

    public bool loop;
}
