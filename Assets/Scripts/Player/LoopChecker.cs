using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopChecker : MonoBehaviour
{
    private HudController hudScript;

    private float lastRot, timeSinceLoopDone = 1;

    private bool turnedAround, loopIsBackwards, canResetTimeSinceLoopDone = true;

    void Start()
    {
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
    }

    void Update()
    {
        if (PlayerController.dead) return;

        CheckRotation();

        if (timeSinceLoopDone < 1) timeSinceLoopDone += Time.deltaTime;
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
        if (transform.eulerAngles.z > 90 && loopingBackwards)
        {
            if (canResetTimeSinceLoopDone) ResetTimeSinceLoop();

            if (transform.eulerAngles.z > 160 && timeSinceLoopDone >= 0.7f) LoopDoneWithoutScoringPoints();
        }

        if (transform.eulerAngles.z < 270 && !loopingBackwards)
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
        if (result) turnedAround = false;

        return result; // If true, the previously scored points will be DOUBLED thanks to the loop
    }

    void LoopDoneWithoutScoringPoints()
    {
        turnedAround = false;
        canResetTimeSinceLoopDone = true;

        hudScript.ChangePointsValue(2, transform.position + new Vector3(0.5f, 1, 0), 1, 1);
    }
}
