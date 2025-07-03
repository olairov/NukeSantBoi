using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindGenerator : MonoBehaviour
{
    public WindSpawnSettings spawnSettings;

    private float timeForNextParticle;
    
    public void GenerateWind()
    {
        timeForNextParticle -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextParticle > 0) return;

        float randValue = Random.value;

        if (randValue > 0.75f) Instantiate(spawnSettings.wind1pref, transform);
        else if (randValue > 0.5f) Instantiate(spawnSettings.wind2pref, transform);
        else if (randValue > 0.25f) Instantiate(spawnSettings.wind3pref, transform);
        else Instantiate(spawnSettings.wind4pref, transform);

        timeForNextParticle = Random.Range(spawnSettings.timeIntervalMin, spawnSettings.timeIntervalMax);
    }
}
