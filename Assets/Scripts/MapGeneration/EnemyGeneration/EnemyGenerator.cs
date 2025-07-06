using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public EnemyGenerationSettings generationSettings;

    public MissileGenerator missileGeneratorScript;

    public void GenerateEnemies()
    {
        if (generationSettings.missiles) missileGeneratorScript.GenerateMissile();
    }

    public void FirstGeneration(float startX, float finishX)
    {

    }
}
