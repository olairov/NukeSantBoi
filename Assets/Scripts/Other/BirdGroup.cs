using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdGroup : MonoBehaviour
{
    private ObjectPassingBy myMovingScript;

    private List<float> birdsStartTimes = new List<float>();
    private float timeSinceAppeared, yPos, randDelay, lastYcosCalculatedYAdder;

    private bool finishedStartingBirds;

    void Start()
    {
        myMovingScript = transform.GetComponent<ObjectPassingBy>();

        ChoseStats();
        lastYcosCalculatedYAdder = Mathf.Cos(Time.time + randDelay) / 5;
    }

    private void ChoseStats()
    {
        float distance = Random.Range(0.6f, 1f);
        transform.localScale = Vector3.one * distance;
        transform.GetComponent<ObjectPassingBy>().realPassingSpeed *= distance;

        int childNum = transform.childCount; // Get the num of children before the loop, as while on it, this number is likely to change.

        for (int idx = 0; idx < childNum; idx++)
        {
            if ((transform.GetChild(idx).name.Contains("4") || transform.GetChild(idx).name.Contains("7")) && Random.value > 0.6f)
            {
                transform.GetChild(idx).GetComponent<SpriteRenderer>().enabled = false;
            }

            transform.GetChild(idx).localScale = Vector3.one * Random.Range(0.18f, 0.3f);

            birdsStartTimes.Add(Random.value * 4);
        }

        yPos = Random.Range(1f, 5f);
        if (distance > 0.8f) transform.position = new Vector3(transform.position.x, yPos, 11); // Make the bird be in front of the nearest buildings when they are bigger.
        randDelay = Random.Range(0, 3.14f);
    }

    void Update()
    {
        if (!finishedStartingBirds) StartBirdAnimations();

        UpdateYPos();
    }

    void UpdateYPos()
    {
        float cosCalculatedYAdder = Mathf.Cos(Time.time + randDelay) / 5; // Difference added this way to let ObjectPassingBy calculate the Y in function of the camera pos.

        transform.position += new Vector3(0, cosCalculatedYAdder - lastYcosCalculatedYAdder, 0);

        lastYcosCalculatedYAdder = cosCalculatedYAdder;
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
}
