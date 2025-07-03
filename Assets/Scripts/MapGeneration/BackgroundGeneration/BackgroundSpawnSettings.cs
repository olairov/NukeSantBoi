using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundSpawnSettings", menuName = "SpawnSettings/Background")]
public class BackgroundSpawnSettings : ScriptableObject
{
    public GameObject layer1BackgroundPrefab, layer2BackgroundPrefab, layer3BackgroundPrefab;
    public float layer1SpawnInterval, layer2SpawnInterval, layer3SpawnInterval, firstGenerationSpaceBetweenBackgrounds;
}
