using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParticlesController : MonoBehaviour
{
    [SerializeField] GameObject wind1pref, wind2pref, wind3pref, wind4pref;

    private float timeForNextParticle;
    
    void Update()
    {
        GenerateWindParticles();
    }

    void GenerateWindParticles()
    {
        timeForNextParticle -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextParticle > 0) return;

        float randValue = Random.value;

        if (randValue > 0.75f) Instantiate(wind1pref, transform);
        else if (randValue > 0.5f) Instantiate(wind2pref, transform);
        else if (randValue > 0.25f) Instantiate(wind3pref, transform);
        else Instantiate(wind4pref, transform);

        timeForNextParticle = Random.Range(0.4f, 0.6f);
    }
}
