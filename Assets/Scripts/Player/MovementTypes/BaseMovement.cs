using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMovement : MonoBehaviour
{
    protected float rotationSpeed, movementSpeed, deviationSpeed, downForceWhenBackwardsMagnitude, movementArotationLimit, deviationWavesSpeed,
        deviationWavesAmplitude, deviationWavesDuration, deviationDecaySmoothness, endOfMovementDeviationMultiplier, loopDownForceFadingSpeed;

    protected Transform playerTransform;
    protected Rigidbody2D playerRB;

    private bool isPaused;
    public virtual bool IsPaused
    {
        set { isPaused = value; }
        get { return isPaused; }
    }

    public virtual void Init(Transform transform, Rigidbody2D rb)
    {
        playerTransform = transform;
        playerRB = rb;
    }

    public void SetPlayerStats()
    {
        StatsMovement playerStats = GameObject.Find("Player").GetComponent<StatsMovement>();

        rotationSpeed = playerStats.rotationSpeed;
        movementSpeed = playerStats.movementSpeed;
        deviationSpeed = playerStats.deviationSpeed;
        downForceWhenBackwardsMagnitude = playerStats.downForceWhenBackwardsMagnitude;
        movementArotationLimit = playerStats.movementArotationLimit;
        deviationWavesSpeed = playerStats.deviationWavesSpeed;
        deviationWavesAmplitude = playerStats.deviationWavesAmplitude;
        deviationWavesDuration = playerStats.deviationWavesDuration;
        deviationDecaySmoothness = playerStats.deviationDecaySmoothness;
        endOfMovementDeviationMultiplier = playerStats.endOfMovementDeviationMultiplier;
        loopDownForceFadingSpeed = playerStats.loopDownForceFadingSpeed;
    }

    public abstract void MovementProcess();
}
