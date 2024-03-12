using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBird : MonoBehaviour
{
    private float yPos, randDelay, lastYcosCalculatedYAdder;

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
        UpdateYPos();
    }

    void UpdateYPos()
    {
        float cosCalculatedYAdder = Mathf.Cos(Time.time + randDelay) / 2; // Difference added this way to let ObjectPassingBy calculate the Y in function of the camera pos.

        transform.position += new Vector3(0, cosCalculatedYAdder - lastYcosCalculatedYAdder, 0);

        lastYcosCalculatedYAdder = cosCalculatedYAdder;
    }
}
