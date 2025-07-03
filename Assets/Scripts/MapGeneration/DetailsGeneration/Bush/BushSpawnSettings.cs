using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BushSpawnSettings", menuName = "SpawnSettings/Details/Bush")]
public class BushSpawnSettings : ScriptableObject
{
    public GameObject bushPrefab;
    public float timeIntervalMin, timeIntervalMax, firstGenerationMinDistanceFromEachOther, firstGenerationMaxDistanceFromEachOther, firstGenerationYPos;
}
