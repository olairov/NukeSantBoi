using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FrontBirdSpawnSettings", menuName = "SpawnSettings/Details/FrontBird")]
public class FrontBirdSpawnSettings : ScriptableObject
{
    public GameObject frontBirdPrefab;
    public float timeIntervalMin, timeIntervalMax, spawnHeightMin, spawnHeightMax, spawnZ;
}
