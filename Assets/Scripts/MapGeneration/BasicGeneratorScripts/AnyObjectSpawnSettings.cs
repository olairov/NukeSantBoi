using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnyObjectSpawnSettings", menuName = "SpawnSettings/Any")]
public class AnyObjectSpawnSettings : ScriptableObject
{
    public float timeIntervalMin, timeIntervalMax, firstGenMinDistance, firstGenMaxDistance,
        firstGenMinDistFromRightBorder, firstGenMaxDistFromRightBorder, firstGenDistFromLeftBorder,
        yMin, yMax, zMin, zMax, xMin, xMax, minScale, maxScale, minRotation, maxRotation;

    public bool appearingObject, canAppearFlipped, canAppearDuplicated, onlyFirstGeneration, firstGenIsOnlyManual;
    public List<Vector3> firstGeneratedObjectsPositions = new List<Vector3>();
    public bool ignoreFirstGenXPos, ignoreFirstGenYPos, ignoreFirstGenZPos;
    public string objectName;
}
