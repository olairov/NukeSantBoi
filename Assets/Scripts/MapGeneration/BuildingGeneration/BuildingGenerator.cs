using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public BuildingGenerationSettings generationSettings;

    private float timeToNextGeneration;
    private int buildingsFromDangerousBuilding;

    [SerializeField] Transform defaultBuildingsContainer, wideBuildingsContainer, skyscrapersContainer, warningsContainerCanvas;

    public void GenerateBuilding()
    {
        timeToNextGeneration -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeToNextGeneration > 0) return;

        Dictionary<string, float> buildingProbabilities = GetBuildingProbabilitiesDictionary(generationSettings.buildingTypesProbabilities);
        int percentageAddition = 0;
        foreach (int percentage in buildingProbabilities.Values) percentageAddition += percentage;
        if (Mathf.RoundToInt(percentageAddition) != 100)
        {
            Debug.LogWarning("Building RAW percentages don't add up to 100!, they add up to: " + percentageAddition.ToString());
        }

        // For every dangerous building there is (the ones that can have their percentage changed), the redistribution process will happen
        Dictionary<string, float> adjustedBuildingProbabilities = new Dictionary<string, float>(buildingProbabilities);
        foreach (string dangerousBuildingName in generationSettings.dangerousBuildingsNames)
        {
            adjustedBuildingProbabilities = GetAdjustedBuildingProbabilities(adjustedBuildingProbabilities, dangerousBuildingName);
        }

        percentageAddition = 0;
        foreach (int percentage in adjustedBuildingProbabilities.Values) percentageAddition += percentage;
        if (Mathf.RoundToInt(percentageAddition) != 100)
        {
            Debug.LogWarning("Building ADJUSTED percentages don't add up to 100!, they add up to: " + percentageAddition.ToString());
        }

        Debug.Log(adjustedBuildingProbabilities["default"] + "   " + adjustedBuildingProbabilities["wide"] + "   " + adjustedBuildingProbabilities["skyscraper"]);

        string buildingToGenerate = DecideGeneratedBuilding(adjustedBuildingProbabilities);        
        switch (buildingToGenerate)
        {
            case "default":
                Instantiate(generationSettings.defaultBuildingPrefab, defaultBuildingsContainer);
                break;
            case "wide":
                Instantiate(generationSettings.wideBuildingPrefab, wideBuildingsContainer);
                break;
            case "skyscraper":
                Instantiate(generationSettings.skyscraperPrefab, transform);
                buildingsFromDangerousBuilding = 0;
                if (!PlayerController.dead) Instantiate(generationSettings.warningPrefab, warningsContainerCanvas);
                break;
            default:
                Debug.LogError("Somehow, an unexistent building class elected...");
                break;
        }

        timeToNextGeneration = Random.Range(generationSettings.spawnIntervalMin, generationSettings.spawnIntervalMax);
        buildingsFromDangerousBuilding++;
    }

    Dictionary<string, float> GetBuildingProbabilitiesDictionary(List<SerializableKeyValue> originalList)
    {
        Dictionary<string, float> buildingProbabilitiesList = new Dictionary<string, float>();
        foreach (SerializableKeyValue pair in originalList)
        {
            if (!buildingProbabilitiesList.ContainsKey(pair.key))
            {
                buildingProbabilitiesList.Add(pair.key, pair.value);
            }
            else
            {
                Debug.LogWarning("Duplicated entry in Building Probabilities Dictionary: " + pair.ToString());
            }
        }
        return buildingProbabilitiesList;
    }

    Dictionary<string, float> GetAdjustedBuildingProbabilities(Dictionary<string, float> originalDictionary, string dangerousBuildingName)
    {
        if (!originalDictionary.ContainsKey(dangerousBuildingName))
        {
            Debug.LogError("Searched Dangerous Building does not exist in the Building Probabilities Dictionary: " + dangerousBuildingName);
            return originalDictionary;
        }

        float dangerousBuildingValue = 0;
        foreach (KeyValuePair<string, float> pair in originalDictionary)
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
        foreach (KeyValuePair<string, float> pair in originalDictionary)
        {
            if (pair.Key == dangerousBuildingName) continue;
            receiversPercentageAddition += pair.Value;
        }

        Dictionary<string, float> adjustedBuildingProbabilities = new Dictionary<string, float>(originalDictionary);
        foreach (KeyValuePair<string, float> pair in originalDictionary)
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

    string DecideGeneratedBuilding(Dictionary<string, float> buildingProbabilities)
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

    public void FirstGeneration(float startX, float finishX)
    {
        Vector3 buildingDefaultPos = generationSettings.defaultBuildingPrefab.transform.position;

        for (float actualX = startX; actualX > finishX - 30; actualX -= generationSettings.firstGenerationSpaceBetweenBuildings)
        {
            if (Random.value < 0.7f) Instantiate(generationSettings.defaultBuildingPrefab, new Vector3(actualX, buildingDefaultPos.y, buildingDefaultPos.z), Quaternion.identity, defaultBuildingsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            else Instantiate(generationSettings.wideBuildingPrefab, new Vector2(actualX, buildingDefaultPos.y), Quaternion.identity, wideBuildingsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        }
    }
}
