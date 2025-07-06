using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BuildingGenerationSettings", menuName = "SpawnSettings/Building")]
public class BuildingGenerationSettings : ScriptableObject
{
    public float spawnIntervalMin, spawnIntervalMax, firstGenerationSpaceBetweenBuildings;
    public List<SerializableStringFloat> buildingTypesProbabilities = new List<SerializableStringFloat>();
    public List<string> dangerousBuildingsNames = new List<string>();
    public int minimumNormalBuildingsInBetweenTwoDangerous;
}