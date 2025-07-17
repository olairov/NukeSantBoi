using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DuplicatedObjectSpawnSettings", menuName = "SpawnSettings/Duplicated")]
public class DuplicatedObjectSpawnStats : ScriptableObject
{
    public float chanceOfDuplication;
    public Vector3 minPositionAdder, maxPositionAdder;
    public float minRotationAdder, maxRotationAdder, minSizeMultiplier, maxSizeMultiplier;
    public bool canAppearOnTheOtherSide, canAppearFlipped;
}
