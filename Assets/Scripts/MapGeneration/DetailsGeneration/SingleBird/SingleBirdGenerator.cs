using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBirdGenerator : MonoBehaviour
{
    public SingleBirdSpawnSettings spawnSettings;

    float timeForNextSingleBird;

    public void GenerateSingleBird()
    {
        timeForNextSingleBird -= Time.deltaTime;
        if (timeForNextSingleBird > 0) return;

        Instantiate(spawnSettings.singleBirdPrefab, transform);

        timeForNextSingleBird = Random.Range(spawnSettings.timeIntervalMin, spawnSettings.timeIntervalMax);
    }

    public void FirstGeneration(float startX, float finishX)
    {
        Instantiate(spawnSettings.singleBirdPrefab,
            new Vector3(Random.Range(startX - spawnSettings.firstGenerationDistanceFromRightBorder, finishX + spawnSettings.firstGenerationDistanceFromLeftBorder), 0, spawnSettings.singleBirdPrefab.transform.position.z),
            Quaternion.identity, transform).GetComponent<ObjectPassingBy>().appearingObject = true;
    }
}
