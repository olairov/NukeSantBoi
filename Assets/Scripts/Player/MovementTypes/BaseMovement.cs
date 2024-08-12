using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    protected float rotationSpeed, movementSpeed, deviationSpeed, downForceWhenBackwardsMagnitude, movementArotationLimit, deviationWavesSpeed,
        deviationWavesAmplitude, deviationWavesDuration, deviationDecaySmoothness, endOfMovementDeviationMultiplier;

    private bool isPaused;
    public virtual bool IsPaused
    {
        set { isPaused = value; }
        get { return isPaused; }
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
    }

    public virtual void MovementProcess()
    {

    }
}
