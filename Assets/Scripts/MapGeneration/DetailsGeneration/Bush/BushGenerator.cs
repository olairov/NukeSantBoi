using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushGenerator : MonoBehaviour
{
    public BushSpawnSettings spawnSettings;

    float timeForNextBush;

    public void GenerateBush()
    {
        timeForNextBush -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextBush > 0) return;

        Instantiate(spawnSettings.bushPrefab, transform);

        timeForNextBush = Random.Range(spawnSettings.timeIntervalMin, spawnSettings.timeIntervalMax);
    }


    public void FirstGeneration(float startX, float finishX)
    {
        for (float actualX = startX; actualX > finishX; actualX -= Random.Range(spawnSettings.firstGenerationMinDistanceFromEachOther, spawnSettings.firstGenerationMaxDistanceFromEachOther))
        {
            Instantiate(spawnSettings.bushPrefab, new Vector3(actualX, spawnSettings.firstGenerationYPos, spawnSettings.bushPrefab.transform.position.z), Quaternion.identity, transform).GetComponent<ObjectPassingBy>().appearingObject = true;
        }
    }
}
