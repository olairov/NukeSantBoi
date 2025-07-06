using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdGroup : MonoBehaviour, ResetPoolObject
{
    private List<float> birdsStartTimes = new List<float>(), birdsYPositions = new List<float>();
    private float timeSinceAppeared, yPos, randDelay, lastYcosCalculatedYAdder;

    private int childNum;

    private bool finishedStartingBirds;

    void Start()
    {
        ChooseStats();
    }

    private void ChooseStats()
    {
        float distance = Random.Range(0.6f, 1f);
        transform.localScale = Vector3.one * distance;
        transform.GetComponent<ObjectPassingBy>().realPassingSpeed *= distance;

        childNum = transform.childCount; // Get the num of children before the loop, as while on it, this number is likely to change.

        for (int idx = 0; idx < childNum; idx++)
        {
            Transform childTransform = transform.GetChild(idx);

            // Allow the possibiliy of having a bird group with less members randomly erasing the last ones.
            if ((childTransform.name.Contains("4") || childTransform.name.Contains("7")) && Random.value > 0.6f)
            {
                childTransform.GetComponent<SpriteRenderer>().enabled = false;
            }

            // Making each bird seem individual
            childTransform.localScale = Vector3.one * Random.Range(0.18f, 0.3f);
            childTransform.GetComponent<Animator>().speed = Random.Range(0.7f, 1.3f);

            birdsStartTimes.Add(Random.value * 4);
            birdsYPositions.Add(childTransform.localPosition.y);
        }

        yPos = Random.Range(1f, 5f);
        if (distance > 0.85f) transform.position = new Vector3(transform.position.x, yPos, 11); // Make the bird be in front of the nearest buildings when they are bigger.
        randDelay = Random.Range(0, 3.14f);
        lastYcosCalculatedYAdder = Mathf.Cos(Time.time + randDelay) / 5;
    }

    void Update()
    {
        if (!finishedStartingBirds) StartBirdAnimations();

        UpdateYPos();
        UpdateEveryBirdYPos();
    }

    void UpdateYPos()
    {
        float cosCalculatedYAdder = Mathf.Cos(Time.time + randDelay) / 4; // Difference added this way to let ObjectPassingBy calculate the Y in function of the camera pos.

        transform.position += new Vector3(0, cosCalculatedYAdder - lastYcosCalculatedYAdder, 0);

        lastYcosCalculatedYAdder = cosCalculatedYAdder;
    }

    void UpdateEveryBirdYPos()
    {
        for (int idx = 0; idx < childNum; idx++)
        {
            Transform childTransform = transform.GetChild(idx);
            childTransform.localPosition = new Vector3(childTransform.localPosition.x, birdsYPositions[idx] + Mathf.Cos(Time.time + birdsStartTimes[idx]) / 7, 0);
        }
    }

    private void StartBirdAnimations()
    {
        timeSinceAppeared += Time.deltaTime;

        for (int idx = 0; idx < transform.childCount; idx++)
        {
            if (timeSinceAppeared > birdsStartTimes[idx]) transform.GetChild(idx).GetComponent<Animator>().SetTrigger("Start");
        }

        if (timeSinceAppeared > 4) finishedStartingBirds = true;
    }


    // Reset Pooled Object State

    public void ResetState()
    {
        timeSinceAppeared = 0;
        finishedStartingBirds = false;

        ChooseStats();
    }
}
