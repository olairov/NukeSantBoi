using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissileSpawnSettings", menuName = "SpawnSettings/Enemies/Missile")]
public class MissileSpawnSettings : ScriptableObject
{
    public GameObject missilePrefab;
    public float spawnIntervalMin, spawnIntervalMax;
}
