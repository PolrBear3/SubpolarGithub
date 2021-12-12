using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMmanager : MonoBehaviour
{
    public AudioSource Track1;
    //public AudioSource Track2;
    //public AudioSource Track3;

    public int trackSelector;
    public int trackHistory;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("BGMmanager");
        if(objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        trackSelector = Random.Range(0, 0);

        if(trackSelector == 0)
        {
            Track1.Play();
            trackHistory = 1;
        }
        // elseif(TrackSelector == 1) {Track2.Play(); trackHistory = 2;}
        // elseif(TrackSelector == 2) {Track3.Play(); trackHistory = 3;}
    }

    void Update()
    {
        if (Track1.isPlaying) //== false && Track2.isPlaying == false && Track3.isPlaying == false
        {
            trackSelector = Random.Range(0, 3);

            if (trackSelector == 0 && trackHistory != 1)
            {
                Track1.Play();
            }
            // elseif(TrackSelector == 1 && trackHistory != 2) {Track2.Play(); trackHistory = 2;}
            // elseif(TrackSelector == 2 && trackHistory != 3) {Track3.Play(); trackHistory = 3;}
        }
    }
}
