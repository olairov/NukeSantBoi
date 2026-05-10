using UnityEngine;

public class InGameSound : MonoBehaviour
{
    private AudioSource myAudio;

    // The "speed" is not the speed, is the time it seconds it takes to appear or disappear. Sorry, I don't want to have to rewrite all the values.
    [SerializeField] private float appearingSpeed, disappearingSpeed, disappearingFromSceneTime;
    static public float volumeMultiplier = 1, timeSpeed = 1;
    private float realVolume, vanishingLerp = 1, appearingSmoothlyVolumeMultiplyer, disappearingSmoothlyVolumeMultiplyer = 1, timeWhenStartedDisappearing;
    public float RealVolume
    {
        set { realVolume = value; }
    }

    private bool appearing, disappearing, playerDead;
    static public bool threeDsound = true, allSoundsDisappearing;
    [SerializeField] private bool disableIfNotThreeDSound, disableWhenPlayerDie, ignorePause, ignoreThreeDsound, appearingSmoothly,
        disappearingSmoothlyInEndOfScene, playWhenPlayerDies;

    void Start()
    {
        if (PlayerPrefs.HasKey("GameVolumeValue")) volumeMultiplier = PlayerPrefs.GetFloat("GameVolumeValue");
        if (PlayerPrefs.HasKey("ThreeDAudio") && PlayerPrefs.GetInt("ThreeDAudio") < 1) threeDsound = false;
        else threeDsound = true;

        myAudio = transform.GetComponent<AudioSource>();
        allSoundsDisappearing = false;

        Initialize();
    }

    void Update()
    {
        if (PlayerController.dead && !playerDead)
        {
            if (playWhenPlayerDies && !myAudio.isPlaying)
            {
                myAudio.Play();
            }
            playerDead = true;
        }

        if (appearing) AppearingSmoothlyProcess();
        if (allSoundsDisappearing && disappearingSmoothlyInEndOfScene) AllSoundsDisappearingSmoothlyProcess();
        if (disappearing) DisappearingSmoothlyProcess();

        if (disableWhenPlayerDie && playerDead)
        {
            myAudio.volume = 0;
            return;
        }

        myAudio.volume = realVolume * volumeMultiplier * (ignorePause ? 1 : timeSpeed) * appearingSmoothlyVolumeMultiplyer * disappearingSmoothlyVolumeMultiplyer;

        if (threeDsound)
        {
            if (!ignoreThreeDsound) myAudio.spatialBlend = 1;
        }
        else
        {
            myAudio.spatialBlend = 0;
            if (disableIfNotThreeDSound) myAudio.volume = 0;
        }
        
        if (playerDead)
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
        if (appearingSmoothlyVolumeMultiplyer < 1)
        {
            appearingSmoothlyVolumeMultiplyer += appearingSpeed * Time.unscaledDeltaTime;
        }
        else
        {
            appearingSmoothlyVolumeMultiplyer = 1;
            appearing = false;
        }
    }

    public void DisappearSmoothly()
    {
        disappearing = true;
    }

    void DisappearingSmoothlyProcess()
    {
        disappearingSmoothlyVolumeMultiplyer = timeWhenStartedDisappearing - Time.time + disappearingSpeed;

        if (disappearingSmoothlyVolumeMultiplyer <= 0)
        {
            disappearingSmoothlyVolumeMultiplyer = 0;
            disappearing = false;
        }
    }

    void AllSoundsDisappearingSmoothlyProcess()
    {
        if (disappearingSmoothlyVolumeMultiplyer > 0)
        {
            disappearingSmoothlyVolumeMultiplyer -= disappearingFromSceneTime * Time.unscaledDeltaTime;
        }
        else
        {
            disappearingSmoothlyVolumeMultiplyer = 0;
        }
    }

    public void Initialize()
    {
        realVolume = myAudio.volume;

        if (appearingSmoothly) appearing = true;
        else appearingSmoothlyVolumeMultiplyer = 1;
        disappearingSmoothlyVolumeMultiplyer = 1;

        myAudio.volume = realVolume * volumeMultiplier;
    }
}
