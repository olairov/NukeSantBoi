using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdGroupGenerator : MonoBehaviour
{
    public BirdGroupSpawnSettings spawnSettings;

    float timeForNextBirdGroup;

    public void GenerateBirdGroup()
    {
        timeForNextBirdGroup -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextBirdGroup > 0) return;

        Instantiate(spawnSettings.birdGroupPrefab, transform);

        timeForNextBirdGroup = Random.Range(spawnSettings.timeIntervalMin, spawnSettings.timeIntervalMax);
    }


    public void FirstGeneration(float startX, float finishX)
    {
        Instantiate(spawnSettings.birdGroupPrefab,
            new Vector3(Random.Range(startX - spawnSettings.firstGenerationDistanceFromRightBorder, finishX + spawnSettings.firstGenerationDistanceFromLeftBorder), 0, spawnSettings.birdGroupPrefab.transform.position.z),
            Quaternion.identity, transform).GetComponent<ObjectPassingBy>().appearingObject = true;
    }
}
