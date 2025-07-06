using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyscraperGenerator : MonoBehaviour
{
    public SkyscraperSpawnSettings spawnSettings;
    public ObjectPool warningSignPool, skyscraperPool;

    public GameObject GenerateSkyscraper()
    {
        if (!PlayerController.dead) warningSignPool.GetObject(true);
        return skyscraperPool.GetObject(true);
    }
}
