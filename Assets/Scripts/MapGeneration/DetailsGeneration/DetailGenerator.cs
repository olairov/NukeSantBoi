using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailGenerator : MonoBehaviour
{
    public DetailGenerationSettings generationSettings;

    public WindGenerator windGeneratorScript;
    public CraneGenerator craneGeneratorScript;
    public BushGenerator bushGeneratorScript;
    public BirdGroupGenerator birdGroupGeneratorScript;
    public SingleBirdGenerator singleBirdGeneratorScript;
    public FrontBirdGenerator frontBirdGeneratorScript;

    public void GenerateDetails()
    {
        if (generationSettings.wind) windGeneratorScript.GenerateWind();
        if (generationSettings.cranes) craneGeneratorScript.GenerateCrane();
        if (generationSettings.bushes) bushGeneratorScript.GenerateBush();
        if (generationSettings.birdGroups) birdGroupGeneratorScript.GenerateBirdGroup();
        if (generationSettings.singleBirds) singleBirdGeneratorScript.GenerateSingleBird();
        if (generationSettings.frontBirds) frontBirdGeneratorScript.GenerateFrontBird();
    }

    public void FirstGeneration(float startX, float finishX)
    {
        if (generationSettings.cranes) craneGeneratorScript.FirstGeneration(startX, finishX);
        if (generationSettings.bushes) bushGeneratorScript.FirstGeneration(startX, finishX);
        if (generationSettings.birdGroups) birdGroupGeneratorScript.FirstGeneration(startX, finishX);
        if (generationSettings.singleBirds) singleBirdGeneratorScript.FirstGeneration(startX, finishX);
    }
}
