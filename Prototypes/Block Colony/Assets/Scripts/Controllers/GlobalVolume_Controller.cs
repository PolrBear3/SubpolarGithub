using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolume_Controller : MonoBehaviour
{
    [SerializeField] private Volume _volume;

    private LensDistortion _lensDistortion;


    // MonoBehaviour
    private void Awake()
    {
        _volume.profile.TryGet(out _lensDistortion);
    }


    // Effect Functions
}
