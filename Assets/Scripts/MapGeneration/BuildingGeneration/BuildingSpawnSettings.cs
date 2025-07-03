using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BuildingSpawnSettings", menuName = "SpawnSettings/Building")]
public class BuildingSpawnSettings : ScriptableObject
{
    public GameObject defaultBuildingPrefab, wideBuildingPrefab, skyscraperPrefab, warningPrefab;
    public float spawnIntervalMin, spawnIntervalMax, firstGenerationSpaceBetweenBuildings;
    public Dictionary<string, float> buildingTypesProbabilities = new Dictionary<string, float>();
    public int minimumNormalBuildingsInBetweenTwoSkyscrapers;
}