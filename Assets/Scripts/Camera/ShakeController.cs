using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeController : MonoBehaviour
{
    private Vector2 waveStartPos, targetPos;

    [SerializeField] private float maxRadius, interval, timeShakeLasts, minWaveDistance, rotationScale, xDisplacement;
    private float strengthMultiplier, shakeMoveProgress, shakeTimeLeft, definitiveMaxRadius;

    [SerializeField] private bool unaffectedByStrengthMultiplier;

    private void Start()
    {
        if (unaffectedByStrengthMultiplier)
        {
            SetDefinitiveMaxRadius(1f);
            return;
        }

        if (PlayerPrefs.HasKey("ScreenshakeValue")) SetDefinitiveMaxRadius(PlayerPrefs.GetFloat("ScreenshakeValue"));
        else SetDefinitiveMaxRadius(0.5f);
    }

    void Update()
    {
        EveryFrameShakeProcess();
    }

    public void Shake()
    {
        if (Time.timeSinceLevelLoad < 0.3f) return;

        CreateWave();
        shakeTimeLeft = timeShakeLasts;
    }

    private void EveryFrameShakeProcess()
    {
        if (definitiveMaxRadius <= 0) return;

        if (shakeTimeLeft > 0) UpdatePos();
        else transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    private void UpdatePos()
    {
        UpdateShakeMoveProgress();

        float smoothedMoveProgress = Mathf.Abs(Mathf.Cos(shakeMoveProgress) / 2 - 0.5f);

        transform.localPosition = new Vector3(Mathf.Lerp(waveStartPos.x, targetPos.x, smoothedMoveProgress) + xDisplacement, Mathf.Lerp(waveStartPos.y, targetPos.y, smoothedMoveProgress), transform.localPosition.z);
        
        float adjustedRotationLerping = transform.localPosition.x / (definitiveMaxRadius * 2 * strengthMultiplier) + 0.5f;
        transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Lerp(-rotationScale, rotationScale, adjustedRotationLerping) * (definitiveMaxRadius / maxRadius));
    }

    private void CreateWave()
    {
        if (shakeTimeLeft > 0) strengthMultiplier = shakeTimeLeft / timeShakeLasts;
        else strengthMultiplier = 1;

        waveStartPos = transform.localPosition;

        for (int idx = 0; idx < 10; idx++)
        {
            targetPos = new Vector2(Random.Range(-definitiveMaxRadius, definitiveMaxRadius) * strengthMultiplier, Random.Range(-definitiveMaxRadius, definitiveMaxRadius) * strengthMultiplier);
            if (Mathf.Abs((targetPos - waveStartPos).magnitude) > minWaveDistance * strengthMultiplier) break;
        }

        shakeMoveProgress = 0;
    }

    private void UpdateShakeMoveProgress()
    {
        shakeMoveProgress += Time.unscaledDeltaTime * interval;

        if (shakeMoveProgress > Mathf.PI)
        {
            shakeMoveProgress = Mathf.PI;
            CreateWave();
        }

        shakeTimeLeft -= Time.unscaledDeltaTime;
        if (shakeTimeLeft < 0) shakeTimeLeft = 0;
    }

    public void SetDefinitiveMaxRadius(float multiplier)
    {
        definitiveMaxRadius = maxRadius * multiplier;
    }
}
