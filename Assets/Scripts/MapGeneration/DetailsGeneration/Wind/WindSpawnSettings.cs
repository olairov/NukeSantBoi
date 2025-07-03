using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WindSpawnSettings", menuName = "SpawnSettings/Details/Wind")]
public class WindSpawnSettings : ScriptableObject
{
    public GameObject wind1pref, wind2pref, wind3pref, wind4pref;
    public float timeIntervalMin, timeIntervalMax;
}
