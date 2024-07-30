using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSound : MonoBehaviour
{
    private AudioSource myAudio;

    [SerializeField] private float appearingSpeed;
    static public float volumeMultiplier = 1, timeSpeed = 1;
    private float realVolume, vanishingLerp = 1, appearingOrDisappearingSmoothlyMultiplyer;
    public float SetRealVolume
    {
        set { realVolume = value; }
    }

    private bool appearing;
    static public bool threeDsound = true, disappearing;
    [SerializeField] private bool disableIfNotThreeDSound, disableWhenPlayerDie, ignorePause, ignoreThreeDsound, appearingSmoothly, disappearingSmoothly;

    void Start()
    {
        if (PlayerPrefs.HasKey("GameVolumeValue")) volumeMultiplier = PlayerPrefs.GetFloat("GameVolumeValue");
        if (PlayerPrefs.GetInt("ThreeDAudio") < 1) threeDsound = false;

        myAudio = transform.GetComponent<AudioSource>();
        realVolume = myAudio.volume;

        if (appearingSmoothly) appearing = true;
        else appearingOrDisappearingSmoothlyMultiplyer = 1;
        disappearing = false;

        myAudio.volume = realVolume * volumeMultiplier;
    }

    void Update()
    {
        if (appearing) AppearingSmoothlyProcess();
        if (disappearing && disappearingSmoothly) DisappearingSmoothlyProcess();

        if (disableWhenPlayerDie && PlayerController.dead)
        {
            myAudio.volume = 0;
            return;
        }

        myAudio.volume = realVolume * volumeMultiplier * timeSpeed * appearingOrDisappearingSmoothlyMultiplyer;
        if (ignorePause) myAudio.volume = realVolume * volumeMultiplier * appearingOrDisappearingSmoothlyMultiplyer;

        if (threeDsound)
        {
            if (!ignoreThreeDsound) myAudio.spatialBlend = 1;
        }
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

    void AppearingSmoothlyProcess()
    {
        if (appearingOrDisappearingSmoothlyMultiplyer < 1)
        {
            appearingOrDisappearingSmoothlyMultiplyer += appearingSpeed * Time.unscaledDeltaTime;
        }
        else
        {
            appearingOrDisappearingSmoothlyMultiplyer = 1;
            appearing = false;
        }
    }

    void DisappearingSmoothlyProcess()
    {
        if (appearingOrDisappearingSmoothlyMultiplyer > 0)
        {
            appearingOrDisappearingSmoothlyMultiplyer -= appearingSpeed * Time.unscaledDeltaTime;
        }
        else
        {
            appearingOrDisappearingSmoothlyMultiplyer = 0;
        }
    }
}
