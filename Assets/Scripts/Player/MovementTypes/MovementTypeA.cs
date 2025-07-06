using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTypeA : BaseMovement
{
    private float lastIdleRotation, lastRotationSpeed, timeSinceDeviationStarted, idleRotationBeforeStop;
    private bool isInCenter = true, wasStoppedBefore, inUpperLimit, inLowerLimit;

    public override void MovementProcess()
    {
        if (!IsPaused)
        {
            float movement = CorrectInputs.rawVerticalAxis;
            float idleRotation = playerTransform.eulerAngles.z - 180;

            if (movement != 0) isInCenter = false; // Only with having pressed one moving button once, it surely won't be in the center.
            else
            {
                // If no movement is being added, but it's not in the center, rotate the plane towards the center.
                if (!isInCenter) movement = RotateTowardsCenter(idleRotation);
            }
            lastIdleRotation = idleRotation;

            // If the movement is not in any of the limits or in the very center, it adds the torque it needs.
            if (((movement > 0 && idleRotation > -movementArotationLimit) || (movement < 0 && idleRotation < movementArotationLimit)) && !isInCenter)
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

    float RotateTowardsCenter(float rotation)
    {
        float movement;

        // Detecting if the plane has already gotten to the center so that it stops.
        if ((lastIdleRotation < 0 && rotation >= 0) || (rotation <= 0 && lastIdleRotation > 0))
        {
            isInCenter = true;
            playerTransform.eulerAngles = new Vector3(0, 0, 180);
            return 0;
        }

        // Setting an automatic movement towards center.
        if (rotation > 0) movement = 1;
        else movement = -1;

        return movement;
    }

    void Deviation(float rotationSpeedBeforeStop, float rotationOffset)
    {
        timeSinceDeviationStarted += Time.deltaTime;
        if (timeSinceDeviationStarted >= deviationWavesDuration)
        {
            if (isInCenter) EndlessDeviation();
            return;
        }

        float deviationDecay = -(timeSinceDeviationStarted / deviationWavesDuration) + 1;

        float rotation = Mathf.Sin(timeSinceDeviationStarted * deviationWavesSpeed) * rotationSpeedBeforeStop / deviationWavesSpeed * endOfMovementDeviationMultiplier * deviationDecay - 180 + rotationOffset;

        playerTransform.eulerAngles = new Vector3(0, 0, rotation);
    }

    void EndlessDeviation()
    {
        timeSinceDeviationStarted += Time.deltaTime;

        float rotation = Mathf.Sin((timeSinceDeviationStarted - deviationWavesDuration) * (deviationWavesSpeed / 5)) * deviationWavesAmplitude - 180;
        playerTransform.eulerAngles = new Vector3(0, 0, rotation);
    }
}
