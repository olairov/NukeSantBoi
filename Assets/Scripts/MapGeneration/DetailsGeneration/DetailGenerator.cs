using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailGenerator : MonoBehaviour
{
    public DetailGenerationSettings generationSettings;

    public WindGenerator windGeneratorScript;
    public BirdGroupGenerator birdGroupGeneratorScript;
    public FrontBirdGenerator frontBirdGeneratorScript;
    public List<AnyObjectGenerator> anyObjectGeneratorScripts;

    public void GenerateDetails()
    {
        if (generationSettings.wind) windGeneratorScript.GenerateWind();
        if (generationSettings.birdGroups) birdGroupGeneratorScript.GenerateBirdGroup();
        if (generationSettings.frontBirds) frontBirdGeneratorScript.GenerateFrontBird();
        
        foreach (AnyObjectGenerator anyObjectGenerator in anyObjectGeneratorScripts)
        {
            if (CheckIfDefaultObjectEnabled(anyObjectGenerator)) anyObjectGenerator.GenerateObject();
        }
    }

    public void FirstGeneration(float startX, float finishX)
    {
        if (generationSettings.birdGroups) birdGroupGeneratorScript.FirstGeneration(startX, finishX);

        foreach (AnyObjectGenerator anyObjectGenerator in anyObjectGeneratorScripts)
        {
            if (CheckIfDefaultObjectEnabled(anyObjectGenerator)) anyObjectGenerator.FirstGeneration(startX, finishX);
        }
    }

    bool CheckIfDefaultObjectEnabled(AnyObjectGenerator anyObjectGenerator)
    {
        foreach (string objectName in generationSettings.defaultObjectsThatWillAppear)
        {
            if (anyObjectGenerator.spawnSettings.objectName == objectName) return true;
        }

        return false;
    }
}
