using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeController : MonoBehaviour
{
    private Vector2 waveStartPos, targetPos;

    // Xdisplacement is only used for objects whose X default position is not x;
    [SerializeField] private float maxRadius, shakeSpeed, timeShakeLasts, minWaveDistance, rotationScale, xDisplacement;
    private float strengthMultiplier, shakeMoveProgress, shakeTimeLeft, multipliedMaxRadius, definitiveMaxRadius, definitiveTimeShakeLasts, yVariableDisplacement;
    public float SetYdisplacement
    {
        set { yVariableDisplacement = value; }
    }

    [SerializeField] private bool unaffectedByStrengthMultiplier, infiniteShake, shakeInStart;
    private bool lastRound, finishedShake = true, hasCreatedLastWave;

    private void Start()
    {
        if (unaffectedByStrengthMultiplier)
        {
            SetDefinitiveMaxRadius(1f);
        }
        else
        {
            if (PlayerPrefs.HasKey("ScreenshakeValue")) SetDefinitiveMaxRadius(PlayerPrefs.GetFloat("ScreenshakeValue"));
            else SetDefinitiveMaxRadius(0.5f);
        }

        if (shakeInStart) Shake(1);
    }

    void Update()
    {
        EveryFrameShakeProcess();
    }

    public void Shake(float multiplier) // Called from an external script to start the shake process.
    {
        if (Time.timeSinceLevelLoad < 0.3f && !shakeInStart) return; // For some mysterious reason, sometimes a screenshake is generated in the beginning of the game. Now it's not.

        definitiveMaxRadius = multipliedMaxRadius * multiplier; // To account for different intensity needs depending on the situation Shake was called.
        definitiveTimeShakeLasts = timeShakeLasts * multiplier;

        lastRound = false;
        finishedShake = false;
        hasCreatedLastWave = false;

        CreateWave(false);
        shakeTimeLeft = definitiveTimeShakeLasts;
    }

    private void EveryFrameShakeProcess()
    {
        if (multipliedMaxRadius <= 0) return;

        if (!finishedShake) UpdatePos();
        else
        {
            
        }
    }

    private void UpdatePos()
    {
        UpdateShakeMoveProgress();

        // This creates a smooth interpolation from [0 - 1] by using the Cos function of a variable with interval [0 - PI]. Then it's divided by two, so that instead of an
        // interval of [-1 - 1], we have [-0.5 - 0.5]. Then it would be reasonalbe to add 0.5 to get an interval of [0 - 1], but that would make the Cos wave to start on the
        // highest point, instead of the lower one. To understand how this solves the problem, view the function in https://www.geogebra.org/classic.
        float smoothedMoveProgress = (Mathf.Cos(shakeMoveProgress) / 2 - 0.5f) * -1;

        transform.localPosition = new Vector3(Mathf.Lerp(waveStartPos.x, targetPos.x, smoothedMoveProgress) + xDisplacement, Mathf.Lerp(waveStartPos.y, targetPos.y, smoothedMoveProgress) + yVariableDisplacement, transform.localPosition.z);
        
        // X position divided by DefinitiveMaxRadius is a float with range [-1 - 1]. The objective is to get a float from [0 - 1], where 0 is the maximum left position and
        // 1 is the maximum right pos. To do this, the divisor is doubled, so that the result is halved. This gives a float from [0.5 - 0.5]. Adding a 0.5 gives what we need.
        float adjustedRotationLerping = transform.localPosition.x - xDisplacement / (definitiveMaxRadius * 2 * strengthMultiplier) + 0.5f;

        // (DefinitiveMaxRadius / MaxRadius) is a float with range (0 - 1]. Multiplying it by the rest only makes the rotation intensity lower depending on the modificated MaxRadius.
        // At some point I did something that increased the intensity of the rotation, so I now half it.
        transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Lerp(-rotationScale, rotationScale, adjustedRotationLerping) * (definitiveMaxRadius / maxRadius) * 0.5f);
    }

    private void UpdateShakeMoveProgress()
    {
        shakeMoveProgress += Time.unscaledDeltaTime * shakeSpeed;

        if (shakeMoveProgress >= Mathf.PI)
        {
            shakeMoveProgress = Mathf.PI;
            CreateWave(false);
        }

        if (!infiniteShake && !hasCreatedLastWave)
        {
            shakeTimeLeft -= Time.unscaledDeltaTime;
            if (shakeTimeLeft <= 0)
            {
                shakeTimeLeft = 0;
                CreateWave(true);
            }
        }
    }

    private void CreateWave(bool isLast)
    {
        if (lastRound)
        {
            finishedShake = true;
            return;
        }

        if (shakeTimeLeft > 0 && !infiniteShake) strengthMultiplier = shakeTimeLeft / definitiveTimeShakeLasts;
        else strengthMultiplier = 1;

        waveStartPos = transform.localPosition - new Vector3(xDisplacement, yVariableDisplacement);

        if (!isLast)
        {
            // A random position for the next TargetPos is chosen, repeating the process if it is too close to the starting pos.
            for (int idx = 0; idx < 10; idx++) // The loop is finite so that in case it never finds a position far enough away, it doesn't freeze.
            {
                targetPos = new Vector2(Random.Range(-definitiveMaxRadius, definitiveMaxRadius) * strengthMultiplier, Random.Range(-definitiveMaxRadius, definitiveMaxRadius) * strengthMultiplier);
                if (Mathf.Abs((targetPos - waveStartPos).magnitude) > minWaveDistance * strengthMultiplier) break;
            }
        }
        else
        {
            targetPos = Vector2.zero;
            lastRound = true;
            hasCreatedLastWave = true;
        }

        shakeMoveProgress = 0;
    }

    public void SetDefinitiveMaxRadius(float multiplier) // To account for shake intensity configuration.
    {
        multipliedMaxRadius = maxRadius * multiplier;
    }
}
