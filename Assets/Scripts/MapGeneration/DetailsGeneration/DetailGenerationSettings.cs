using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DetailsGenerationSettings", menuName = "SpawnSettings/Detail")]
public class DetailGenerationSettings : ScriptableObject
{
    public bool wind, cranes, bushes, birdGroups, singleBirds, frontBirds;
}
