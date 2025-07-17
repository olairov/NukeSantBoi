using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : ObjectGenerator
{
    public BuildingGenerationSettings generationSettings;

    public DefaultBuildingGenerator defaultBuildingGeneratorScript;
    public DefaultBuildingGenerator wideBuildingGeneratorScript; // Uses "DefaultBuildingGenerator" because Wide doesn't need an individual script.
    public SkyscraperGenerator skyscraperGeneratorScript;

    Dictionary<string, float> buildingProbabilities;
    private float timeToNextGeneration;
    private int buildingsFromDangerousBuilding;

    private void Start()
    {
        buildingProbabilities = GetBuildingProbabilitiesDictionary(generationSettings.buildingTypesProbabilities);
    }

    Dictionary<string, float> GetBuildingProbabilitiesDictionary(List<SerializableStringFloat> originalList)
    {
        Dictionary<string, float> buildingProbabilitiesList = new Dictionary<string, float>();
        foreach (SerializableStringFloat pair in originalList)
        {
            if (!buildingProbabilitiesList.ContainsKey(pair.key)) buildingProbabilitiesList.Add(pair.key, pair.value);
            else Debug.LogWarning("Duplicated entry in Building Probabilities Dictionary: " + pair.ToString());
        }
        return buildingProbabilitiesList;
    }


    public override void GenerateObject()
    {
        timeToNextGeneration -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeToNextGeneration > 0) return;

        // Checking if the DEFAULT BUILDING PROBABILITIES add up to 100. If not, this would be a USER'S MISTAKE.
        float percentageAddition = 0;
        foreach (float percentage in buildingProbabilities.Values) percentageAddition += percentage;
        if (Mathf.RoundToInt(percentageAddition) != 100)
        {
            Debug.LogWarning("Building RAW percentages don't add up to 100!, they add up to: " + percentageAddition.ToString());
        }

        Dictionary<string, float> adjustedBuildingProbabilities = AdjustBuildingProbabilities(buildingProbabilities, generationSettings.dangerousBuildingsNames);

        // Checking if the ADJUSTED BUILDING PROBABILITIES add up to 100. If not, this would be a CODE ERROR.
        percentageAddition = 0;
        foreach (float percentage in adjustedBuildingProbabilities.Values) percentageAddition += percentage;
        if (Mathf.RoundToInt(percentageAddition) != 100)
        {
            Debug.LogWarning("Building ADJUSTED percentages don't add up to 100!, they add up to: " + percentageAddition.ToString());
        }

        string chosenBuilding = DecideBuildingToGenerate(adjustedBuildingProbabilities);
        GenerateBuildingWithAdjustedProbabilities(chosenBuilding, generationSettings.dangerousBuildingsNames);

        timeToNextGeneration = Random.Range(generationSettings.spawnIntervalMin, generationSettings.spawnIntervalMax);
        buildingsFromDangerousBuilding++;
    }

    // Adjusting All Buildings' Probabilities --->

    Dictionary<string, float> AdjustBuildingProbabilities(Dictionary<string, float> originalProbabilities, List<string> dangerousBuildings)
    {
        // For every dangerous building there is (the ones that can have their percentage changed), the redistribution process will happen.
        Dictionary<string, float> adjustedBuildingProbabilities = new Dictionary<string, float>(originalProbabilities);
        foreach (string dangerousBuildingName in dangerousBuildings)
        {
            adjustedBuildingProbabilities = AdjustDangerousBuildingProbability(adjustedBuildingProbabilities, dangerousBuildingName);
        }

        return adjustedBuildingProbabilities;
    }

    Dictionary<string, float> AdjustDangerousBuildingProbability(Dictionary<string, float> originalProbabilities, string dangerousBuildingName)
    {
        if (!originalProbabilities.ContainsKey(dangerousBuildingName))
        {
            Debug.LogError("Searched Dangerous Building does not exist in the Building Probabilities Dictionary: " + dangerousBuildingName);
            return originalProbabilities;
        }

        float dangerousBuildingValue = 0;
        // Getting the target dangerous building's default probability of being elected.
        foreach (KeyValuePair<string, float> pair in originalProbabilities)
        {
            if (pair.Key == dangerousBuildingName)
            {
                dangerousBuildingValue = pair.Value;
                break;
            }
        }

        // The chance a dangerous building appears isn't only dictated by the percentage chance, but also depends on external factors.
        float realChanceItIsDangerousBuilding = buildingsFromDangerousBuilding > generationSettings.minimumNormalBuildingsInBetweenTwoDangerous * ObjectPassingBy.realSpeedMultiplier ?
            dangerousBuildingValue / Mathf.Sqrt(ObjectPassingBy.realSpeedMultiplier) : 0;

        // When "realChanceItIsSkyscraper" is lower than "chanceItIsSkyscraper", there is a small percentage left
        // that is not taken into account when calculating the rest of the buildings' probabilities.
        float percentageVoid = dangerousBuildingValue - realChanceItIsDangerousBuilding;

        // The sum of the percentage of all the buildings that will have the "percentage void" distributed proportionally
        // is used later to calculate each building's weight in between the receivers, so that each one gets added a
        // fraction of the "percentage void" proportional to their original percentage.
        float receiversPercentageAddition = 0;
        foreach (KeyValuePair<string, float> pair in originalProbabilities)
        {
            if (pair.Key == dangerousBuildingName) continue;
            receiversPercentageAddition += pair.Value;
        }

        Dictionary<string, float> adjustedBuildingProbabilities = new Dictionary<string, float>(originalProbabilities);
        foreach (KeyValuePair<string, float> pair in originalProbabilities)
        {
            if (pair.Key == dangerousBuildingName)
            {
                adjustedBuildingProbabilities[dangerousBuildingName] = realChanceItIsDangerousBuilding;
                continue;
            }

            // The weight of this building type within the receiver building classes is calculated.
            // It is a float from 0 to 1, being 1 the total percentage of all the receivers.
            float buildingProbabilityWeight = pair.Value / receiversPercentageAddition;

            // With this multiplication, the fraction of "percentage void" that belongs to this building is calculated.
            float percentageToAddToThisBuildingClass = percentageVoid * buildingProbabilityWeight;

            adjustedBuildingProbabilities[pair.Key] += percentageToAddToThisBuildingClass;
        }

        return adjustedBuildingProbabilities;
    }

    // <--- Adjusting All Buildings' Probabilities

    // Using the adjusted percentages to randomly choose a building:
    string DecideBuildingToGenerate(Dictionary<string, float> buildingProbabilities)
    {
        float randomValue = Random.value * 100;
        // For every building probability that is checked, that probability is added here until it reaches 100
        // (in that case, a building class will already have been elected).
        float maxCheckedRandomValue = 0;

        KeyValuePair<string, float> lastCheckedPair = new KeyValuePair<string, float>();
        foreach (KeyValuePair<string, float> pair in buildingProbabilities)
        {
            maxCheckedRandomValue += pair.Value;
            if (randomValue < maxCheckedRandomValue) return pair.Key;
            lastCheckedPair = pair;
        }

        // In case the remote chance that randomValue is exactly 100, it will exit the loop without returning,
        // but it should count as if the last element of the list had been elected.
        return lastCheckedPair.Key;
    }

    // Actually spawning the building, when it has been elected:
    GameObject GenerateBuildingWithAdjustedProbabilities(string buildingToGenerate, List<string> dangerousBuildings)
    {
        foreach (string buildingClass in dangerousBuildings)
        {
            if (buildingToGenerate == buildingClass) buildingsFromDangerousBuilding = 0;
        }

        switch (buildingToGenerate)
        {
            case "default":
                return defaultBuildingGeneratorScript.GenerateBuilding();
            case "wide":
                return wideBuildingGeneratorScript.GenerateBuilding();
            case "skyscraper":
                return skyscraperGeneratorScript.GenerateSkyscraper();
            default:
                Debug.LogError("Somehow, an unexistent building class elected...");
                return null;
        }
    }

    

    public override void FirstGeneration(float startX, float finishX)
    {
        Dictionary<string, float> adjustedBuildingProbabilities = AdjustBuildingProbabilities(buildingProbabilities, generationSettings.dangerousBuildingsNames);

        for (float actualX = startX; actualX > finishX - 30; actualX -= generationSettings.firstGenerationSpaceBetweenBuildings)
        {
            string chosenBuilding = DecideBuildingToGenerate(adjustedBuildingProbabilities);
            GameObject generatedBuilding = GenerateBuildingWithAdjustedProbabilities(chosenBuilding, generationSettings.dangerousBuildingsNames);

            generatedBuilding.transform.position = new Vector3(actualX, generatedBuilding.transform.position.y, generatedBuilding.transform.position.z);
            generatedBuilding.GetComponent<ObjectPassingBy>().dontSetPosition = true;
        }
    }
}
