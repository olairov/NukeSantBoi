using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTypeC : BaseMovement
{
    private float lastIdleRotation, lastRotationSpeed, timeSinceDeviationStarted, downForceWhenBackwards;
    private int lastNotZeroInputMovement;
    private bool isInCenter = true, wasStoppedBefore;

    private PlayerController playerScript;

    public override void Init(Transform transform, Rigidbody2D rb)
    {
        base.Init(transform, rb);
        playerScript = transform.GetComponent<PlayerController>();
    }

    public override void MovementProcess()
    {
        if (!IsPaused)
        {
            int movement = CorrectInputs.rawVerticalAxis;
            if (movement != 0) lastNotZeroInputMovement = movement;
            float idleRotation = playerTransform.eulerAngles.z - 180;

            if (movement != 0) isInCenter = false; // Only with having pressed one moving button once, it surely won't be in the center.
            else
            {
                // If no movement is being added, but it's not in the center, rotate the plane towards the center.
                if (!isInCenter) movement = RotateTowardsCenter(idleRotation, lastNotZeroInputMovement);
            }
            lastIdleRotation = idleRotation;

            float multiplierInCaseOfFrontFlip = 1;
            if (movement > 0 && playerTransform.eulerAngles.z < 150)
            {
                multiplierInCaseOfFrontFlip = Mathf.Cos((playerTransform.eulerAngles.z + 75) / 47.75f) * 0.4f + 1;
                // Basically, the more straight-down you are facing, the more slower you'll be able to rotate forward.
                // I divide the angles by 47.75 instead of 57.3 (180 / pi, explained why in a comment below) so that the amplitude of the wave is 300 units,
                // what divided into two is 150, instead of 360 units. I do that because I want that the curve starts in 0 and finishes in 150.

                playerTransform.position += new Vector3(0, (multiplierInCaseOfFrontFlip - 1) * 4 * Time.deltaTime, 0);
                //pushing the ship down so that it's difficult to do a frontflip.
            }

            if (!isInCenter) // Always tha plane is not in the center it'll rotate either by the player or automatically to the center.
            {
                playerRB.AddTorque(-movement * multiplierInCaseOfFrontFlip * rotationSpeed * Time.deltaTime);

                wasStoppedBefore = false;
            }
            else // Otherwise, it stops completely and adds the deviation effect with the same rotation force it had before stopping.
            {
                NotRotatingProcess();
            }
        }
        float forceToAddFormula = Mathf.Cos(-playerTransform.eulerAngles.z / 57.3f - Mathf.PI / 2);

        LoopDownForce();

        Vector3 forceToAdd = new Vector3(0, forceToAddFormula * movementSpeed * ((ObjectPassingBy.speedMultiplier - 1) / 2.5f + 1) * Time.deltaTime, 0);
        playerTransform.position += forceToAdd;
    }

    void LoopDownForce()
    {
        float additionDownForceWhenBackwards = Mathf.Cos((playerTransform.eulerAngles.z - 180) / 57.3f) * 0.8f - 0.2f;
        // I divide the rotation between 57.3 (180 / pi) so that the amplitude of the wave is 360 units, instead of 2pi units, to representate it in degrees.

        bool lastDownForceIsTooBig = downForceWhenBackwards < -4;

        if (additionDownForceWhenBackwards > 0) additionDownForceWhenBackwards *= loopDownForceFadingSpeed;
        downForceWhenBackwards += additionDownForceWhenBackwards * downForceWhenBackwardsMagnitude * Time.deltaTime;
        if (downForceWhenBackwards > 0) downForceWhenBackwards = 0;

        playerTransform.position += new Vector3(0, downForceWhenBackwards * Time.deltaTime, 0);

        if (downForceWhenBackwards < -4 && !lastDownForceIsTooBig) playerScript.LosingSpeedWarning(true);
        else if (downForceWhenBackwards >= -4 && lastDownForceIsTooBig) playerScript.LosingSpeedWarning(false);
    }

    void NotRotatingProcess()
    {
        if (!wasStoppedBefore)
        {
            lastRotationSpeed = playerRB.angularVelocity;
            timeSinceDeviationStarted = 0;
            playerRB.angularVelocity = 0;
            
            wasStoppedBefore = true;
        }

        Deviation(lastRotationSpeed);
    }

    int RotateTowardsCenter(float rotation, int inputMovement)
    {
        int movement;

        // Detecting if the plane has already gotten to the center so that it stops.
        if ((lastIdleRotation < 0 && rotation >= 0 && inputMovement < 0) || (rotation <= 0 && lastIdleRotation > 0 && inputMovement > 0))
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

    void Deviation(float rotationSpeedBeforeStop)
    {
        timeSinceDeviationStarted += Time.deltaTime;
        if (timeSinceDeviationStarted >= deviationWavesDuration)
        {
            if (isInCenter) EndlessDeviation();
            return;
        }

        float deviationDecay = -(timeSinceDeviationStarted / deviationWavesDuration) + 1;

        float rotation = Mathf.Sin(timeSinceDeviationStarted * deviationWavesSpeed) * rotationSpeedBeforeStop / deviationWavesSpeed * endOfMovementDeviationMultiplier * deviationDecay - 180;

        playerTransform.eulerAngles = new Vector3(0, 0, rotation);
    }

    void EndlessDeviation()
    {
        timeSinceDeviationStarted += Time.deltaTime;

        float rotation = Mathf.Sin((timeSinceDeviationStarted - deviationWavesDuration) * (deviationWavesSpeed / 5)) * deviationWavesAmplitude - 180;
        playerTransform.eulerAngles = new Vector3(0, 0, rotation);
    }
}
