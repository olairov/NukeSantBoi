using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSound : MonoBehaviour
{
    private AudioSource myAudio;

    static public float volumeMultiplier = 1, timeSpeed = 1;
    private float realVolume, vanishingLerp = 1;

    static public bool threeDsound = true;
    [SerializeField] private bool disableIfNotThreeDSound, disableWhenPlayerDie, ignorePause, ignoreThreeDsound;

    void Start()
    {
        if (PlayerPrefs.HasKey("GameVolumeValue")) volumeMultiplier = PlayerPrefs.GetFloat("GameVolumeValue");
        if (PlayerPrefs.GetInt("ThreeDAudio") < 1) threeDsound = false;

        myAudio = transform.GetComponent<AudioSource>();
        realVolume = myAudio.volume;

        myAudio.volume = realVolume * volumeMultiplier;
    }

    void Update()
    {
        if (disableWhenPlayerDie && PlayerController.dead)
        {
            myAudio.volume = 0;
            return;
        }

        myAudio.volume = realVolume * volumeMultiplier * timeSpeed;
        if (ignorePause) myAudio.volume = realVolume * volumeMultiplier;

        if (threeDsound) { if (!ignorePause) myAudio.spatialBlend = 1; }
        else
        {
            myAudio.spatialBlend = 0;
            if (disableIfNotThreeDSound) myAudio.volume = 0;
        }
        if (PlayerController.dead)
        {
            myAudio.spatialBlend = 0;

            if (disableIfNotThreeDSound)
            {
                vanishingLerp -= Time.unscaledDeltaTime * 0.5f;
                myAudio.volume *= vanishingLerp;
            }
        }
    }
}
