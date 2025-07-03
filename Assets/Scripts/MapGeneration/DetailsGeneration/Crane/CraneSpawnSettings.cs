using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraneSpawnSettings", menuName = "SpawnSettings/Details/Crane")]
public class CraneSpawnSettings : ScriptableObject
{
    public GameObject cranePrefab;
    public float timeIntervalMin, timeIntervalMax, firstGenerationDistanceFromRightBorder, firstGenerationDistanceFromLeftBorder;
}
