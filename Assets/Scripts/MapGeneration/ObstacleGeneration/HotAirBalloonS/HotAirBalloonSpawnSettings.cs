using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HotAirBalloonSpawnSettings", menuName = "SpawnSettings/Obstacles/HotAirBalloon")]
public class HotAirBalloonSpawnSettings : ScriptableObject
{
    public GameObject hotAirBalloonPrefab;
    public float spawnIntervalMin, spawnIntervalMax, minSpawnDistanceFromLowerLimit, minSpawnDistanceFromUpperLimit, distanceFromDuplicatedBalloon,
        firstGenerationMinX, firstGenerationMinDistanceFromRightBorder, firstGenerationMinY;
}
