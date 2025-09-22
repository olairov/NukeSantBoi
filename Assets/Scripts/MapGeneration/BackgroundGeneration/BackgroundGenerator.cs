using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : ObjectGenerator
{
    public BackgroundSpawnSettings spawnSettings;
    
    List<float> timeForEveryNextLayer = new List<float>();

    float numberOfLayers;
    bool canGenerateBackground;

    private void Awake()
    {
        numberOfLayers = transform.childCount;
        canGenerateBackground = spawnSettings.spawnTimeIntervals.Count >= numberOfLayers;
        if (!canGenerateBackground) Debug.LogError("Number of Background Layers is not the same within Background information lists.");
        else timeForEveryNextLayer = GetResetedTimesList();
    }

    public override void GenerateObject()
    {
        for (int idx = 0; idx < numberOfLayers; idx++)
        {
            timeForEveryNextLayer[idx] -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
            if (timeForEveryNextLayer[idx] > 0) continue;

            EveryLayerBackgroundGeneration(idx);
        }
    }

    GameObject EveryLayerBackgroundGeneration(int idx)
    {
        timeForEveryNextLayer[idx] = spawnSettings.spawnTimeIntervals[idx];

        GameObject instantiatedBackground = transform.GetChild(idx).GetComponent<ObjectPool>().GetObject(true);
        return instantiatedBackground;
    }

    public override void FirstGeneration(float startX, float finishX)
    {
        for (float actualX = startX + 5; actualX > finishX - 30; actualX -= spawnSettings.firstGenerationSpaceBetweenBackgrounds)
        {
            for (int idx = 0; idx < numberOfLayers; idx++)
            {
                GameObject instantiatedBackground = EveryLayerBackgroundGeneration(idx);

                instantiatedBackground.transform.position = new Vector3(actualX, 0, instantiatedBackground.transform.position.z);
                instantiatedBackground.GetComponent<ObjectPassingBy>().DontSetPosition = true;
            }
        }

        timeForEveryNextLayer = GetResetedTimesList();
    }

    List<float> GetResetedTimesList()
    {
        List<float> resetedList = new List<float>();

        for (int idx = 0; idx < numberOfLayers; idx++)
        {
            resetedList.Add(0);
        }

        return resetedList;
    }
}
