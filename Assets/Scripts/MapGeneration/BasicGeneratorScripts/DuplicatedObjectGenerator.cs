using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicatedObjectGenerator : MonoBehaviour
{
    [SerializeField] DuplicatedObjectSpawnStats spawnSettings;

    public void DuplicateObject(Transform objectToDuplicate)
    {
        if (Random.value * 100 > spawnSettings.chanceOfDuplication) return;

        Transform duplicatedObject = objectToDuplicate.GetComponent<PooledObject>().myObjectPool.GetObject(false).transform;

        Vector3 originalObjectPos = objectToDuplicate.position;
        ObjectPassingBy originalObjectPassingByScript = objectToDuplicate.GetComponent<ObjectPassingBy>();
        if (originalObjectPassingByScript != null && !originalObjectPassingByScript.DontSetPosition)
        {
            float xPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x + originalObjectPassingByScript.AppearingDistance;
            originalObjectPos = new Vector3(xPos, originalObjectPos.y, originalObjectPos.z);
        }

        float xPosAdder = Random.Range(spawnSettings.minPositionAdder.x, spawnSettings.maxPositionAdder.x);
        float yPosAdder = Random.Range(spawnSettings.minPositionAdder.y, spawnSettings.maxPositionAdder.y);
        float zPosAdder = Random.Range(spawnSettings.minPositionAdder.z, spawnSettings.maxPositionAdder.z);
        if (spawnSettings.canAppearOnTheOtherSide && Random.value > 0.5f) xPosAdder *= -1;
        Vector3 position = originalObjectPos + new Vector3(xPosAdder, yPosAdder, zPosAdder);

        Vector3 rotation = objectToDuplicate.eulerAngles + new Vector3(0, 0, Random.Range(spawnSettings.minRotationAdder, spawnSettings.maxRotationAdder));

        Vector3 scale = objectToDuplicate.localScale * Random.Range(spawnSettings.minSizeMultiplier, spawnSettings.maxSizeMultiplier);
        if (spawnSettings.minSizeMultiplier == 0 && spawnSettings.maxSizeMultiplier == 0) scale = objectToDuplicate.localScale;
        if (spawnSettings.canAppearFlipped && Random.value > 0.5f) scale = new Vector3(-scale.x, scale.y, scale.z);

        ObjectPassingBy objectPassingByScript = duplicatedObject.GetComponent<ObjectPassingBy>();
        if (objectPassingByScript != null) objectPassingByScript.DontSetPosition = true;

        if (duplicatedObject.GetComponent<PooledObject>().alreadyUsed)
        {
            foreach (var poolable in duplicatedObject.GetComponents<ResetPoolObject>())
            {
                poolable.Initialize();
            }
        }

        duplicatedObject.position = position;
        duplicatedObject.eulerAngles = rotation;
        duplicatedObject.localScale = scale;
    }
}
