using UnityEngine;

public class AnyObjectGenerator : ObjectGenerator
{
    public ObjectPool myObjectPool;
    public AnyObjectSpawnSettings spawnSettings;
    public DuplicatedObjectGenerator duplicatorScript;

    float timeForNextObject, realTimeIntervalMin, realTimeIntervalMax, realFirstGenMinDistance, realFirstGenMaxDistance;

    private void Awake()
    {
        if (myObjectPool == null) myObjectPool = GetComponent<ObjectPool>();
        if (duplicatorScript == null) duplicatorScript = GetComponent<DuplicatedObjectGenerator>();

        if ((spawnSettings.timeIntervalMin + spawnSettings.timeIntervalMax) / 2 < 0.1f)
        {
            Debug.LogWarning(transform.name + "'s Generation Rate is too FAST! Objects being generated every 0.1 seconds.");
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
            Debug.LogWarning(transform.name + "'s First Generation Distance is too SHORT! Objects being generated every 0.1 units.");
            realFirstGenMinDistance = 0.1f;
            realFirstGenMaxDistance = 0.1f;
        }
        else
        {
            realFirstGenMinDistance = spawnSettings.firstGenMinDistance;
            realFirstGenMaxDistance = spawnSettings.firstGenMaxDistance;
        }
    }

    public override void GenerateObject()
    {
        timeForNextObject -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (spawnSettings.onlyFirstGeneration || timeForNextObject > 0) return;

        CreateObject(
                new Vector3(Random.Range(spawnSettings.xMin, spawnSettings.xMax),
                Random.Range(spawnSettings.yMin, spawnSettings.yMax),
                Random.Range(spawnSettings.zMin, spawnSettings.zMax)),
                !spawnSettings.appearingObject, false, false, spawnSettings.appearingObject);

        timeForNextObject = Random.Range(realTimeIntervalMin, realTimeIntervalMax);
    }


    public override void FirstGeneration(float startX, float finishX)
    {
        ManualFirstGeneration();
        if (spawnSettings.firstGenIsOnlyManual) return;

        float realStartX = startX - Random.Range(spawnSettings.firstGenMinDistFromRightBorder, spawnSettings.firstGenMaxDistFromRightBorder);
        float realFinishX = finishX + spawnSettings.firstGenDistFromLeftBorder;

        for (float actualX = realStartX; actualX > realFinishX; actualX -= Random.Range(realFirstGenMinDistance, realFirstGenMaxDistance))
        {
            CreateObject(
                new Vector3(actualX, Random.Range(spawnSettings.yMin, spawnSettings.yMax),
                Random.Range(spawnSettings.zMin, spawnSettings.zMax)),
                false, false, false, true);
        }
    }

    void ManualFirstGeneration()
    {
        foreach (Vector3 pos in spawnSettings.firstGeneratedObjectsPositions)
        {
            Vector3 realPos = new Vector3(pos.x,
                spawnSettings.ignoreFirstGenYPos ? Random.Range(spawnSettings.yMin, spawnSettings.yMax) : pos.y,
                spawnSettings.ignoreFirstGenZPos ? Random.Range(spawnSettings.zMin, spawnSettings.zMax) : pos.z);
            
            CreateObject(realPos, false, false, false, true);
        }
    }

    void CreateObject(Vector3 pos, bool ignoreXPos, bool ignoreYPos, bool ignoreZPos, bool appearingObject)
    {
        Transform instantiatedObject = myObjectPool.GetObject(false).transform;

        if (appearingObject)
        {
            ObjectPassingBy objectPassingByScript = instantiatedObject.GetComponent<ObjectPassingBy>();
            if (objectPassingByScript == null)
            {
                Debug.LogWarning("ObjectPassingBy script NOT Found in " + instantiatedObject.name + "!");
            }
            else objectPassingByScript.dontSetPosition = true;
        }

        float xPos = ignoreXPos ? instantiatedObject.position.x : pos.x;
        float yPos = ignoreYPos ? instantiatedObject.position.y : pos.y;
        float zPos = ignoreZPos ? instantiatedObject.position.z : pos.z;
        instantiatedObject.position = new Vector3(xPos, yPos, zPos);

        instantiatedObject.eulerAngles = new Vector3(instantiatedObject.eulerAngles.x, instantiatedObject.eulerAngles.y,
            Random.Range(spawnSettings.minRotation, spawnSettings.maxRotation));

        Vector3 defaultScale = new Vector3(spawnSettings.canAppearFlipped && Random.value > 0.5f ? -1 : 1, 1, 1);
        if (spawnSettings.minScale != 0 || spawnSettings.maxScale != 0) defaultScale *= Random.Range(spawnSettings.minScale, spawnSettings.maxScale);
        instantiatedObject.localScale = defaultScale;

        if (instantiatedObject.GetComponent<PooledObject>().alreadyUsed)
        {
            foreach (var poolable in instantiatedObject.GetComponents<ResetPoolObject>())
            {
                poolable.Initialize();
            }
        }

        if (spawnSettings.canAppearDuplicated) duplicatorScript.DuplicateObject(instantiatedObject.transform);
    }
}
