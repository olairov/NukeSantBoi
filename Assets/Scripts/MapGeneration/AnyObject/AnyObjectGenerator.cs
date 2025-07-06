using UnityEngine;

public class AnyObjectGenerator : MonoBehaviour
{
    public ObjectPool myObjectPool;
    public AnyObjectSpawnSettings spawnSettings;

    float timeForNextObject, realTimeIntervalMin, realTimeIntervalMax, realFirstGenMinDistance, realFirstGenMaxDistance;

    private void Awake()
    {
        if (myObjectPool == null) myObjectPool = GetComponent<ObjectPool>();

        if ((spawnSettings.timeIntervalMin + spawnSettings.timeIntervalMax) / 2 < 0.1f)
        {
            Debug.LogWarning(transform.name + "'s Generation rate is too FAST! Objects being generated every 0.1 seconds.");
            realTimeIntervalMin = 0.1f;
            realTimeIntervalMax = 0.1f;
        }
        else
        {
            realTimeIntervalMin = spawnSettings.timeIntervalMin;
            realTimeIntervalMax = spawnSettings.timeIntervalMax;
        }

        if (!spawnSettings.firstGenIsOnlyManual && (spawnSettings.firstGenMinDistance + spawnSettings.firstGenMaxDistance) / 2 < 0.1f)
        {
            realFirstGenMinDistance = spawnSettings.firstGenMinDistance;
            realFirstGenMaxDistance = spawnSettings.firstGenMaxDistance;
        }
    }

    public void GenerateObject()
    {
        timeForNextObject -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (spawnSettings.onlyFirstGeneration || timeForNextObject > 0) return;

        GenerateObject(
                new Vector3(Random.Range(spawnSettings.xMin, spawnSettings.xMax),
                Random.Range(spawnSettings.yMin, spawnSettings.yMax),
                Random.Range(spawnSettings.zMin, spawnSettings.zMax)),
                !spawnSettings.appearingObject, false, false);

        timeForNextObject = Random.Range(realTimeIntervalMin, realTimeIntervalMax);
    }


    public void FirstGeneration(float startX, float finishX)
    {
        ManualFirstGeneration();
        if (spawnSettings.firstGenIsOnlyManual) return;

        float realStartX = startX - Random.Range(spawnSettings.firstGenMinDistFromRightBorder, spawnSettings.firstGenMaxDistFromRightBorder);
        float realFinishX = finishX + spawnSettings.firstGenDistFromLeftBorder;

        for (float actualX = realStartX; actualX > realFinishX; actualX -= Random.Range(realFirstGenMinDistance, realFirstGenMaxDistance))
        {
            GenerateObject(
                new Vector3(actualX, Random.Range(spawnSettings.yMin, spawnSettings.yMax),
                Random.Range(spawnSettings.zMin, spawnSettings.zMax)),
                false, false, false);
        }
    }

    void ManualFirstGeneration()
    {
        foreach (Vector3 pos in spawnSettings.firstGeneratedObjectsPositions)
        {
            Vector3 realPos = new Vector3(pos.x,
                spawnSettings.ignoreFirstGenYPos ? Random.Range(spawnSettings.yMin, spawnSettings.yMax) : pos.y,
                spawnSettings.ignoreFirstGenZPos ? Random.Range(spawnSettings.zMin, spawnSettings.zMax) : pos.z);
            
            GenerateObject(realPos, false, false, false);
        }
    }

    void GenerateObject(Vector3 pos, bool ignoreXPos, bool ignoreYPos, bool ignoreZPos)
    {
        GameObject instantiatedObject = myObjectPool.GetObject(true);

        float xPos = ignoreXPos ? instantiatedObject.transform.position.x : pos.x;
        float yPos = ignoreYPos ? instantiatedObject.transform.position.y : pos.y;
        float zPos = ignoreZPos ? instantiatedObject.transform.position.z : pos.z;

        instantiatedObject.transform.position = new Vector3(xPos, yPos, zPos);

        if (!spawnSettings.appearingObject) return;

        ObjectPassingBy script = instantiatedObject.GetComponent<ObjectPassingBy>();
        if (script == null)
        {
            Debug.LogWarning("ObjectPassingBy script NOT Found in " + instantiatedObject.name + "!");
            return;
        }
        script.dontSetPosition = true;
    }
}
