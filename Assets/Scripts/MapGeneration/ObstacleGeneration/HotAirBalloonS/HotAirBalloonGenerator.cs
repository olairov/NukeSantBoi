using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotAirBalloonGenerator : MonoBehaviour
{
    public HotAirBalloonSpawnSettings spawnSettings;

    private float timeToNextGeneration;

    public void GenerateHotAirBalloon()
    {
        timeToNextGeneration -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeToNextGeneration > 0) return;

        GameObject hotAirBalloon = Instantiate(spawnSettings.hotAirBalloonPrefab, transform);

        float yPos = Random.Range(
            Camera.main.ScreenToWorldPoint(Vector3.zero).y + spawnSettings.minSpawnDistanceFromLowerLimit,
            Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - spawnSettings.minSpawnDistanceFromUpperLimit);
        hotAirBalloon.transform.position = new Vector3(hotAirBalloon.transform.position.x, yPos, transform.position.z);


        if (Random.value > 0.9f)
        {
            float yPosDuplicated = transform.position.y < 0 ? transform.position.y + spawnSettings.distanceFromDuplicatedBalloon : transform.position.y - spawnSettings.distanceFromDuplicatedBalloon;
            Vector3 otherPos = new Vector3(0, yPosDuplicated, transform.position.z);
            Instantiate(spawnSettings.hotAirBalloonPrefab, otherPos, Quaternion.identity, transform);
        }

        timeToNextGeneration = Random.Range(spawnSettings.spawnIntervalMin, spawnSettings.spawnIntervalMax);
    }

    public void FirstGeneration()
    {
        GameObject hotAirBalloon = Instantiate(spawnSettings.hotAirBalloonPrefab, transform);
        hotAirBalloon.GetComponent<ObjectPassingBy>().dontSetPosition = true;

        float xPos = Random.Range(spawnSettings.firstGenerationMinX,
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x) - spawnSettings.firstGenerationMinDistanceFromRightBorder;

        float yPos = Random.Range(spawnSettings.firstGenerationMinY,
            Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - spawnSettings.minSpawnDistanceFromUpperLimit);

        hotAirBalloon.transform.position = new Vector3(xPos, yPos, transform.position.z);
    }
}
