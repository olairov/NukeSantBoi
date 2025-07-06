using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public ObstacleGenerationSettings generationSettings;

    public HotAirBalloonGenerator hotAirBalloonGeneratorScript;

    public void GenerateObstacles()
    {
        if (generationSettings.hotAirBalloons) hotAirBalloonGeneratorScript.GenerateHotAirBalloon();
    }

    public void FirstGeneration(float startX, float finishX)
    {
        if (generationSettings.hotAirBalloons) hotAirBalloonGeneratorScript.FirstGeneration();
    }
}
