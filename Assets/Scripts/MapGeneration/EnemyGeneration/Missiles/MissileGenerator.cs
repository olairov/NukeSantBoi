using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileGenerator : MonoBehaviour
{
    public MissileSpawnSettings spawnSettings;

    float timeForNextMissile;

    public void GenerateMissile()
    {
        timeForNextMissile -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextMissile > 0) return;

        Instantiate(spawnSettings.missilePrefab, transform);

        timeForNextMissile = Random.Range(spawnSettings.spawnIntervalMin, spawnSettings.spawnIntervalMax);
    }
}
