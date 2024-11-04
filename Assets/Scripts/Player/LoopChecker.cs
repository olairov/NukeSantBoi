using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopChecker : MonoBehaviour
{
    private HudController hudScript;

    private Rigidbody2D rb;

    private float lastRot;

    private bool turnedAround, turnedTowardsDown;

    void Start()
    {
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (PlayerController.dead) return;

        CheckLoop();
    }

    void CheckLoop()
    {
        float rotationDifference = lastRot - transform.eulerAngles.z;

        if (Mathf.Abs(rotationDifference) > 180)
        {
            turnedAround = !turnedAround;

            if (rotationDifference > 180) turnedTowardsDown = true;
            else if (rotationDifference < -180) turnedTowardsDown = false;
        }

        if (turnedAround)
        {
            if (transform.eulerAngles.z > 150 && turnedTowardsDown)
            {
                AddPointsWhenLoop();
                turnedAround = false;
            }
            if (transform.eulerAngles.z < 210 && !turnedTowardsDown)
            {
                AddPointsWhenLoop();
                turnedAround = false;
            }
        }

        lastRot = transform.eulerAngles.z;
    }

    void AddPointsWhenLoop()
    {
        hudScript.ChangePointsValue(2, transform.position + new Vector3(0, 1, 0), 1);
    }
}
