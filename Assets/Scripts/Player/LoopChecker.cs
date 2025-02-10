using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopChecker : MonoBehaviour
{
    private HudController hudScript;
    private AudioSource loopSound, epicLoopSound;

    private float lastRot, timeSinceLoopDone = 1, timeSinceVolumeBlendStarted = 1, originalVolume;

    [SerializeField] private int pointsForNormalLoop;

    private bool turnedAround, loopIsBackwards, canResetTimeSinceLoopDone = true;

    void Start()
    {
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();

        loopSound = transform.Find("Sounds/NormalLoop").GetComponent<AudioSource>();
        epicLoopSound = transform.Find("Sounds/EpicLoop").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (PlayerController.dead) return;

        CheckRotation();

        if (timeSinceLoopDone < 1) timeSinceLoopDone += Time.deltaTime;

        if (timeSinceVolumeBlendStarted < 1) VolumeBlend();
    }

    void CheckRotation()
    {
        float rotationDifference = lastRot - transform.eulerAngles.z;

        if (Mathf.Abs(rotationDifference) > 180)
        {
            turnedAround = !turnedAround;

            loopIsBackwards = rotationDifference > 180;
        }

        if (turnedAround) LoopDetection(loopIsBackwards);

        lastRot = transform.eulerAngles.z;
    }

    void LoopDetection(bool loopingBackwards)
    {
        if (transform.eulerAngles.z > 60 && loopingBackwards)
        {
            if (canResetTimeSinceLoopDone) ResetTimeSinceLoop();

            if (transform.eulerAngles.z > 160 && timeSinceLoopDone >= 0.7f) LoopDoneWithoutScoringPoints();
        }

        if (transform.eulerAngles.z < 300 && !loopingBackwards)
        {
            if (canResetTimeSinceLoopDone) ResetTimeSinceLoop();

            if (transform.eulerAngles.z < 200 && timeSinceLoopDone >= 0.7f) LoopDoneWithoutScoringPoints();
        }
    }

    void ResetTimeSinceLoop()
    {
        timeSinceLoopDone = 0;
        canResetTimeSinceLoopDone = false;
    }

    public bool PointsScored()
    {
        bool result = timeSinceLoopDone < 1f && turnedAround;
        if (result)
        {
            turnedAround = false;
            epicLoopSound.Play();
            timeSinceVolumeBlendStarted = 0;
            originalVolume = AudioListener.volume;
        }

        return result; // If true, the previously scored points will be DOUBLED thanks to the loop
    }

    void LoopDoneWithoutScoringPoints()
    {
        turnedAround = false;
        canResetTimeSinceLoopDone = true;

        loopSound.Play();
        hudScript.ChangePointsValue(pointsForNormalLoop, transform.position + new Vector3(0.5f, 1, 0), 1, 1);
    }

    void VolumeBlend()
    {
        timeSinceVolumeBlendStarted += Time.unscaledDeltaTime;

        // actualVolume = sen(-x * π) / 2 + 1     --> Copypaste it into https://www.geogebra.org/classic
        float actualVolumeMultiplier = Mathf.Sin(-timeSinceVolumeBlendStarted * Mathf.PI) / 2 + 1;

        if (actualVolumeMultiplier > 1) AudioListener.volume = originalVolume;
        else AudioListener.volume = originalVolume * actualVolumeMultiplier;
    }
}
