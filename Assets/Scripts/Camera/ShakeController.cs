using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeController : MonoBehaviour
{
    private Vector2 waveStartPos, targetPos;

    // Xdisplacement is only used for objects whose X default position is not x;
    [SerializeField] private float maxRadius, shakeSpeed, timeShakeLasts, minWaveDistance, rotationScale, xDisplacement;
    private float strengthMultiplier, shakeMoveProgress, shakeTimeLeft, definitiveMaxRadius, yVariableDisplacement;
    public float SetYdisplacement
    {
        set { yVariableDisplacement = value; }
    }

    [SerializeField] private bool unaffectedByStrengthMultiplier, infiniteShake, shakeInStart;
    private bool lastRound, finishedShake;

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

        if (shakeInStart) Shake();
    }

    void Update()
    {
        EveryFrameShakeProcess();
    }

    public void Shake() // Called from the outside to start the shake process.
    {
        if (Time.timeSinceLevelLoad < 0.3f && !shakeInStart) return; // For some mysterious reason, sometimes a screenshake is generated in the beginning of the game.

        lastRound = false;
        finishedShake = false;

        CreateWave(false);
        shakeTimeLeft = timeShakeLasts;
    }

    private void EveryFrameShakeProcess()
    {
        if (definitiveMaxRadius <= 0) return;

        if (!finishedShake) UpdatePos();
        else
        {
            transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
    }

    private void UpdatePos()
    {
        UpdateShakeMoveProgress();

        float smoothedMoveProgress = Mathf.Abs(Mathf.Cos(shakeMoveProgress) / 2 - 0.5f);

        transform.localPosition = new Vector3(Mathf.Lerp(waveStartPos.x, targetPos.x, smoothedMoveProgress) + xDisplacement, Mathf.Lerp(waveStartPos.y, targetPos.y, smoothedMoveProgress) + yVariableDisplacement, transform.localPosition.z);
        
        float adjustedRotationLerping = transform.localPosition.x / (definitiveMaxRadius * 2 * strengthMultiplier) + 0.5f;
        transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Lerp(-rotationScale, rotationScale, adjustedRotationLerping) * (definitiveMaxRadius / maxRadius));
    }

    private void UpdateShakeMoveProgress()
    {
        shakeMoveProgress += Time.unscaledDeltaTime * shakeSpeed;

        if (shakeMoveProgress > Mathf.PI * 2)
        {
            shakeMoveProgress = Mathf.PI * 2;
            CreateWave(false);
        }

        if (!infiniteShake) shakeTimeLeft -= Time.unscaledDeltaTime;
        if (shakeTimeLeft <= 0)
        {
            if (lastRound)
            {
                finishedShake = true;
                return;
            }

            shakeTimeLeft = 0;
            CreateWave(true);
        }
    }

    private void CreateWave(bool isLast)
    {
        if (shakeTimeLeft > 0 && !infiniteShake) strengthMultiplier = shakeTimeLeft / timeShakeLasts;
        else strengthMultiplier = 1;

        waveStartPos = transform.localPosition - new Vector3(0, yVariableDisplacement);

        if (!isLast)
        {
            for (int idx = 0; idx < 10; idx++)
            {
                targetPos = new Vector2(Random.Range(-definitiveMaxRadius, definitiveMaxRadius) * strengthMultiplier, Random.Range(-definitiveMaxRadius, definitiveMaxRadius) * strengthMultiplier);
                if (Mathf.Abs((targetPos - waveStartPos).magnitude) > minWaveDistance * strengthMultiplier) break;
            }
        }
        else
        {
            targetPos = Vector2.zero;
            lastRound = true;
        }

        shakeMoveProgress = 0;
    }

    public void SetDefinitiveMaxRadius(float multiplier)
    {
        definitiveMaxRadius = maxRadius * multiplier;
    }
}
