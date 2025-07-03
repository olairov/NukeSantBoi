using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    public BackgroundSpawnSettings spawnSettings;
    
    public Transform layer1Container, layer2Container, layer3Container;

    float timeForNextLayer1, timeForNextLayer2, timeForNextLayer3;

    public void GenerateBackground()
    {
        Layer1BackgroundGeneration();
        Layer2BackgroundGeneration();
        Layer3BackgroundGeneration();
    }

    void Layer1BackgroundGeneration()
    {
        timeForNextLayer1 -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextLayer1 > 0) return;

        Instantiate(spawnSettings.layer1BackgroundPrefab, layer1Container);

        timeForNextLayer1 = spawnSettings.layer1SpawnInterval;
    }

    void Layer2BackgroundGeneration()
    {
        timeForNextLayer2 -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextLayer2 > 0) return;

        Instantiate(spawnSettings.layer2BackgroundPrefab, layer2Container);

        timeForNextLayer2 = spawnSettings.layer2SpawnInterval;
    }

    void Layer3BackgroundGeneration()
    {
        timeForNextLayer3 -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextLayer3 > 0) return;

        Instantiate(spawnSettings.layer3BackgroundPrefab, layer3Container);

        timeForNextLayer3 = spawnSettings.layer3SpawnInterval;
    }


    public void FirstGeneration(float startX, float finishX)
    {
        for (float actualX = startX + 5; actualX > finishX - 30; actualX -= spawnSettings.firstGenerationSpaceBetweenBackgrounds)
        {
            Instantiate(spawnSettings.layer1BackgroundPrefab, new Vector2(actualX, 0), Quaternion.identity, layer1Container).GetComponent<ObjectPassingBy>().appearingObject = true;
            Instantiate(spawnSettings.layer2BackgroundPrefab, new Vector2(actualX, 0), Quaternion.identity, layer2Container).GetComponent<ObjectPassingBy>().appearingObject = true;
            Instantiate(spawnSettings.layer3BackgroundPrefab, new Vector2(actualX, 0), Quaternion.identity, layer3Container).GetComponent<ObjectPassingBy>().appearingObject = true;
        }
    }
}
