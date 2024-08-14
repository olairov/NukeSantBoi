using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTypeB : BaseMovement
{
    private float lastIdleRotation, lastRotationSpeed, timeSinceDeviationStarted, idleRotationBeforeStop;
    private bool wasStoppedBefore, inUpperLimit, inLowerLimit;

    private Transform playerTransform;
    private Rigidbody2D playerRB;

    public MovementTypeB(Transform transform, Rigidbody2D rb)
    {
        playerTransform = transform;
        playerRB = rb;
    }

    public override void MovementProcess()
    {
        if (!IsPaused)
        {
            float movement = CorrectInputs.rawVerticalAxis;
            float idleRotation = playerTransform.eulerAngles.z - 180;

            // If the movement is not in any of the limits or in the very center, it adds the torque it needs.
            if (((movement > 0 && idleRotation > -movementArotationLimit) || (movement < 0 && idleRotation < movementArotationLimit)) && movement != 0)
            {
                if ((movement < 0 && inUpperLimit) || (movement > 0 && inLowerLimit))
                {
                    Deviation(lastRotationSpeed, idleRotationBeforeStop);
                }
                else
                {
                    playerRB.AddTorque(-movement * rotationSpeed * Time.deltaTime);

                    inUpperLimit = false;
                    inLowerLimit = false;
                    wasStoppedBefore = false;
                }
            }
            else // Otherwise, it stops completely and adds the deviation effect with the same rotation force it had before stopping.
            {
                NotRotatingProcess(idleRotation);
            }

            lastIdleRotation = idleRotation;
        }
        float forceToAddFormula = Mathf.Cos(-playerTransform.eulerAngles.z / 57.3f - Mathf.PI / 2);

        Vector3 forceToAdd = new Vector3(0, forceToAddFormula * movementSpeed * ((ObjectPassingBy.speedMultiplier - 1) / 2.5f + 1) * Time.deltaTime, 0);
        playerTransform.position += forceToAdd;
    }

    void NotRotatingProcess(float idleRotation)
    {
        if (!wasStoppedBefore)
        {
            lastRotationSpeed = playerRB.angularVelocity;
            idleRotationBeforeStop = playerTransform.eulerAngles.z - 180;
            timeSinceDeviationStarted = 0;

            playerRB.angularVelocity = 0;
            if (idleRotation >= movementArotationLimit)
            {
                playerTransform.eulerAngles = new Vector3(0, 0, movementArotationLimit - 180);
                inUpperLimit = true;
            }
            if (idleRotation <= -movementArotationLimit)
            {
                playerTransform.eulerAngles = new Vector3(0, 0, -movementArotationLimit - 180);
                inLowerLimit = true;
            }

            wasStoppedBefore = true;
        }

        Deviation(lastRotationSpeed, idleRotationBeforeStop);
    }

    void Deviation(float rotationSpeedBeforeStop, float rotationOffset)
    {
        timeSinceDeviationStarted += Time.deltaTime;
        if (timeSinceDeviationStarted >= deviationWavesDuration)
        {
            EndlessDeviation(rotationOffset);
            return;
        }

        float deviationDecay = -(timeSinceDeviationStarted / deviationWavesDuration) + 1;

        float rotation = Mathf.Sin(timeSinceDeviationStarted * deviationWavesSpeed) * rotationSpeedBeforeStop / deviationWavesSpeed * endOfMovementDeviationMultiplier * deviationDecay - 180 + rotationOffset;

        playerTransform.eulerAngles = new Vector3(0, 0, rotation);
    }

    void EndlessDeviation(float rotationOffset)
    {
        timeSinceDeviationStarted += Time.deltaTime;

        float rotation = Mathf.Sin((timeSinceDeviationStarted - deviationWavesDuration) * (deviationWavesSpeed / 5)) * deviationWavesAmplitude - 180 + rotationOffset;
        playerTransform.eulerAngles = new Vector3(0, 0, rotation);
    }
}
