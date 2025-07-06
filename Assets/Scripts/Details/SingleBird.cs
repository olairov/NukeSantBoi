using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBird : MonoBehaviour
{
    private float randDelay, lastCosCalculatedYAdder;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        randDelay = Random.Range(0, 3.14f);

        float distance = Random.Range(0.15f, 0.4f); // Closer the "distance" value, further away the bird will be. (sorry)

        transform.localScale = transform.localScale * distance;
        if (distance > 0.35f) transform.position = new Vector3(transform.position.x, transform.position.y, 11); // Make the bird be in front of the nearest buildings when they are bigger.
        transform.GetComponent<ObjectPassingBy>().realPassingSpeed *= Mathf.Pow(distance * 3, 2);
    }

    void Update()
    {
        UpdateYPos();
    }

    void UpdateYPos()
    {
        float cosCalculatedYAdder = Mathf.Cos(Time.time + randDelay) / 2; // Difference added this way to let ObjectPassingBy calculate the Y in function of the camera pos.

        transform.position += new Vector3(0, cosCalculatedYAdder - lastCosCalculatedYAdder, 0);

        lastCosCalculatedYAdder = cosCalculatedYAdder;
    }


    // Reset Pooled Object State

    public void ResetState()
    {
        lastCosCalculatedYAdder = 0;

        Initialize();
    }
}
