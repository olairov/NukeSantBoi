using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTypeD : BaseMovement
{
    private Transform playerTransform;
    private Rigidbody2D playerRB;

    private PlayerController playerScript;

    private float downForceWhenBackwards, rotationVelocityBeforeStop, idleRotationBeforeStop, timeSinceDeviationStarted;

    public MovementTypeD(Transform transform, Rigidbody2D rb)
    {
        playerTransform = transform;
        playerRB = rb;

        playerScript = transform.GetComponent<PlayerController>();
    }

    public override void MovementProcess()
    {
        if (!IsPaused)
        {
            float movement = CorrectInputs.rawVerticalAxis;

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

            playerRB.AddTorque(-movement * multiplierInCaseOfFrontFlip * rotationSpeed * Time.deltaTime);

            if (movement == 0)
            {
                if (CorrectInputs.justReleasedVertical)
                {
                    rotationVelocityBeforeStop = playerRB.angularVelocity;
                    idleRotationBeforeStop = playerTransform.eulerAngles.z - 180;
                    timeSinceDeviationStarted = 0;
                }

                Deviation(rotationVelocityBeforeStop, idleRotationBeforeStop);
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
