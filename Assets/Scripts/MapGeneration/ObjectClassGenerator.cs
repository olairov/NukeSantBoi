using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClassGenerator : ObjectGenerator
{
    [SerializeField] List<ObjectGenerator> generatorScripts = new();

    public bool generatesEnemies, stopWhenPlayerDies;

    public override void GenerateObject()
    {
        if (stopWhenPlayerDies && PlayerController.dead || generatesEnemies && !MapGenerator.canGenerateEnemies) return;

        foreach (ObjectGenerator generatorScript in generatorScripts)
        {
            generatorScript.GenerateObject();
        }
    }

    public override void FirstGeneration(float startX, float finishX)
    {
        if (generatesEnemies && !MapGenerator.canGenerateEnemies) return;

        foreach (ObjectGenerator generatorScript in generatorScripts)
        {
            generatorScript.FirstGeneration(startX, finishX);
        }
    }
}
