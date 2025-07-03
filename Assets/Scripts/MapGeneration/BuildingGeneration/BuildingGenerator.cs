using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public BuildingSpawnSettings spawnSettings;

    private float timeToNextGeneration;
    private int buildingsFromDangerousBuilding;

    [SerializeField] Transform defaultBuildingsContainer, wideBuildingsContainer, skyscrapersContainer, warningsContainerCanvas;

    public void GenerateBuilding()
    {
        timeToNextGeneration -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeToNextGeneration > 0) return;

        Dictionary<string, float> adjustedBuildingProbabilities = GetAdjustedBuildingProbabilities(spawnSettings.buildingTypesProbabilities);

        string buildingToGenerate = DecideGeneratedBuilding(adjustedBuildingProbabilities);        
        switch (buildingToGenerate)
        {
            case "default":
                Instantiate(spawnSettings.defaultBuildingPrefab, defaultBuildingsContainer);
                break;
            case "wide":
                Instantiate(spawnSettings.wideBuildingPrefab, wideBuildingsContainer);
                break;
            case "skyscraper":
                Instantiate(spawnSettings.skyscraperPrefab, transform);
                buildingsFromDangerousBuilding = 0;
                if (!PlayerController.dead) Instantiate(spawnSettings.warningPrefab, warningsContainerCanvas);
                break;
            default:
                Debug.LogError("Somehow, an unexistent building class elected...");
                break;
        }

        timeToNextGeneration = Random.Range(spawnSettings.spawnIntervalMin, spawnSettings.spawnIntervalMax);
        buildingsFromDangerousBuilding++;
    }

    Dictionary<string, float> GetAdjustedBuildingProbabilities(Dictionary<string, float> originalDictionary)
    {
        KeyValuePair<string, float> skyscraperPair = new KeyValuePair<string, float>();
        foreach (KeyValuePair<string, float> pair in originalDictionary)
        {
            if (pair.Key == "skyscraper")
            {
                skyscraperPair = pair;
                break;
            }
        }
        if (skyscraperPair.Value == 0) return originalDictionary;

        // The chance a skyscraper appears isn't only dictated by the percentage chance, but also depends on external factors.
        float realChanceItIsSkyscraper = buildingsFromDangerousBuilding > spawnSettings.minimumNormalBuildingsInBetweenTwoSkyscrapers * ObjectPassingBy.realSpeedMultiplier ?
            0 : skyscraperPair.Value / Mathf.Sqrt(ObjectPassingBy.speedMultiplier);

        // When "realChanceItIsSkyscraper" is lower than "chanceItIsSkyscraper", there is a small percentage left
        // that is not taken into account when calculating the rest of the buildings' probabilities.
        float percentageVoid = skyscraperPair.Value - realChanceItIsSkyscraper;

        // The sum of the percentage of all the buildings that will have the "percentage void" distributed proportionally
        // is used later to calculate each building's weight in between the receivers, so that each one gets added a
        // fraction of the "percentage void" proportional to their original percentage.
        float receiversPercentageAddition = 0;
        foreach (KeyValuePair<string, float> pair in originalDictionary)
        {
            if (pair.Key != "skyscraper") continue;
            receiversPercentageAddition += pair.Value;
        }

        Dictionary<string, float> adjustedBuildingProbabilities = originalDictionary;
        foreach (KeyValuePair<string, float> pair in originalDictionary)
        {
            if (pair.Key != "skyscraper")
            {
                adjustedBuildingProbabilities["skyscraper"] = realChanceItIsSkyscraper;
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
            if (maxCheckedRandomValue < randomValue) return pair.Key;
            lastCheckedPair = pair;
        }

        // In case the remote chance that randomValue is exactly 100, it will exit the loop without returning,
        // but it should count as if the last element of the list had been elected.
        return lastCheckedPair.Key;
    }

    public void FirstGeneration(float startX, float finishX)
    {
        Vector3 buildingDefaultPos = spawnSettings.defaultBuildingPrefab.transform.position;

        for (float actualX = startX; actualX > finishX - 30; actualX -= spawnSettings.firstGenerationSpaceBetweenBuildings)
        {
            if (Random.value < 0.7f) Instantiate(spawnSettings.defaultBuildingPrefab, new Vector3(actualX, buildingDefaultPos.y, buildingDefaultPos.z), Quaternion.identity, defaultBuildingsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            else Instantiate(spawnSettings.wideBuildingPrefab, new Vector2(actualX, buildingDefaultPos.y), Quaternion.identity, wideBuildingsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        }
    }
}
