using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneGenerator : MonoBehaviour
{
    public CraneSpawnSettings spawnSettings;

    float timeForNextCrane;

    public void GenerateCrane()
    {
        timeForNextCrane -= Time.deltaTime;
        if (timeForNextCrane > 0) return;

        Instantiate(spawnSettings.cranePrefab, transform);

        timeForNextCrane = Random.Range(spawnSettings.timeIntervalMin, spawnSettings.timeIntervalMax);
    }


    public void FirstGeneration(float startX, float finishX)
    {
        Instantiate(spawnSettings.cranePrefab,
            new Vector3(Random.Range(startX - spawnSettings.firstGenerationDistanceFromRightBorder, finishX + spawnSettings.firstGenerationDistanceFromLeftBorder), 0, spawnSettings.cranePrefab.transform.position.z),
            Quaternion.identity, transform).GetComponent<ObjectPassingBy>().appearingObject = true;
    }
}
