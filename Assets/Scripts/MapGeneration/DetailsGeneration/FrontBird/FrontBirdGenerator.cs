using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontBirdGenerator : MonoBehaviour
{
    public FrontBirdSpawnSettings spawnSettings;

    float timeForNextFrontBird;

    public void GenerateFrontBird()
    {
        timeForNextFrontBird -= Time.deltaTime;
        if (timeForNextFrontBird > 0) return;

        Instantiate(spawnSettings.frontBirdPrefab, new Vector3(0, Random.Range(spawnSettings.spawnHeightMin, spawnSettings.spawnHeightMax), spawnSettings.spawnZ), Quaternion.identity, transform);

        timeForNextFrontBird = Random.Range(spawnSettings.timeIntervalMin, spawnSettings.timeIntervalMax);
    }
}
