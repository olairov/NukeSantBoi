using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSound : MonoBehaviour
{
    private AudioSource myAudio;

    static public float volumeMultiplier = 0.7f, timeSpeed = 1;
    private float realVolume, blurrLerp = 1;
    [SerializeField] private float blurrSpeed;

    void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolumeValue")) volumeMultiplier = PlayerPrefs.GetFloat("MusicVolumeValue");

        myAudio = transform.GetComponent<AudioSource>();
        realVolume = myAudio.volume;

        myAudio.volume = realVolume * volumeMultiplier;
    }

    void Update()
    {
        myAudio.volume = realVolume * volumeMultiplier * timeSpeed;
        if (PlayerController.dead) SoundBlurr();
    }

    void SoundBlurr()
    {
        blurrLerp -= Time.unscaledDeltaTime * blurrSpeed;

        myAudio.pitch = blurrLerp;
        myAudio.volume = realVolume * volumeMultiplier * timeSpeed * blurrLerp;
    }
}
