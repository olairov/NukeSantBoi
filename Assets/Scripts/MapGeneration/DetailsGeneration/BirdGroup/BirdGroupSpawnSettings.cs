using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BirdGroupSpawnSettings", menuName = "SpawnSettings/Details/BirdGroup")]
public class BirdGroupSpawnSettings : ScriptableObject
{
    public GameObject birdGroupPrefab;
    public float timeIntervalMin, timeIntervalMax, firstGenerationDistanceFromRightBorder, firstGenerationDistanceFromLeftBorder;
}
