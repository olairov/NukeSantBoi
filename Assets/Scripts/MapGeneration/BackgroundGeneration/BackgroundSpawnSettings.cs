using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundSpawnSettings", menuName = "SpawnSettings/Background")]
public class BackgroundSpawnSettings : ScriptableObject
{
    public List<float> spawnTimeIntervals;
    public float firstGenerationSpaceBetweenBackgrounds;
}
