using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISound : MonoBehaviour
{
    private AudioSource myAudio;

    static public float volumeMultiplier = 1;
    float realVolume;

    void Start()
    {
        if (PlayerPrefs.HasKey("InterfaceVolumeValue")) volumeMultiplier = PlayerPrefs.GetFloat("InterfaceVolumeValue");

        myAudio = transform.GetComponent<AudioSource>();
        realVolume = myAudio.volume;

        myAudio.volume = realVolume * volumeMultiplier;
    }

    void Update()
    {
        myAudio.volume = realVolume * volumeMultiplier;
    }
}
