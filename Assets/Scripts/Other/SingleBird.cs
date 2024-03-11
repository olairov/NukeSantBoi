using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBird : MonoBehaviour
{
    private float yPos, randDelay;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        transform.GetComponent<Animator>().SetTrigger("Start");

        yPos = Random.Range(0f, 5f);
        randDelay = Random.Range(0, 3.14f);

        float distance = Random.Range(0.2f, 0.4f);

        transform.localScale = Vector3.one * distance;
        if (distance > 0.35f) transform.position = new Vector3(transform.position.x, yPos, 11); // Make the bird be in front of the nearest buildings when they are bigger.
        transform.GetComponent<ObjectPassingBy>().realPassingSpeed *= Mathf.Pow(distance * 3, 2);
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, yPos + Mathf.Cos(Time.time + randDelay) / 5, transform.position.z);
    }
}
