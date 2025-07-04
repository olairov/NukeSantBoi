using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BuildingSpawnSettings", menuName = "SpawnSettings/Building")]
public class BuildingGenerationSettings : ScriptableObject
{
    public GameObject defaultBuildingPrefab, wideBuildingPrefab, skyscraperPrefab, warningPrefab;
    public float spawnIntervalMin, spawnIntervalMax, firstGenerationSpaceBetweenBuildings;
    public List<SerializableKeyValue> buildingTypesProbabilities = new List<SerializableKeyValue>();
    public List<string> dangerousBuildingsNames = new List<string>();
    public int minimumNormalBuildingsInBetweenTwoDangerous;
}