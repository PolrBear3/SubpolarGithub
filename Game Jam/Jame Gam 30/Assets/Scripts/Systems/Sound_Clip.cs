using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound_Clip
{
    [HideInInspector] public AudioSource audioSource;

    [HideInInspector] public int soundNum;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    public bool loop;
}
