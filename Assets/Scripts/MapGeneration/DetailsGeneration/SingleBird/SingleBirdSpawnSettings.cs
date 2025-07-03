using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleBirdSpawnSettings", menuName = "SpawnSettings/Details/SingleBird")]
public class SingleBirdSpawnSettings : ScriptableObject
{
    public GameObject singleBirdPrefab;
    public float timeIntervalMin, timeIntervalMax, firstGenerationDistanceFromRightBorder, firstGenerationDistanceFromLeftBorder;
}
