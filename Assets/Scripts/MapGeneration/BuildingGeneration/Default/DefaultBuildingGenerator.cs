using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBuildingGenerator : MonoBehaviour
{
    public DefaultBuildingSpawnSettings spawnSettings;
    public ObjectPool buildingObjectPool;

    private void Awake()
    {
        if (buildingObjectPool == null)
        {
            if (GetComponent<ObjectPool>())
            {
                buildingObjectPool = GetComponent<ObjectPool>();
                return;
            }

            Debug.LogError("Unassigned Object Pool: " + transform.name);
        }
    }

    public GameObject GenerateBuilding()
    {
        return buildingObjectPool.GetObject(true);
    }
}
