using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdGroup : MonoBehaviour
{
    private List<float> birdsStartTimes = new List<float>();
    private float timeSinceAppeared, yPos, randDelay;

    private bool finishedStartingBirds;

    void Start()
    {
        ChoseStats();
    }

    private void ChoseStats()
    {
        transform.localScale = Vector3.one * Random.Range(0.6f, 1f);

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
        randDelay = Random.Range(0, 3.14f);
    }

    void Update()
    {
        if (!finishedStartingBirds) StartBirdAnimations();

        transform.position = new Vector3(transform.position.x, yPos + Mathf.Cos(Time.time + randDelay) / 5, transform.position.z);
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
