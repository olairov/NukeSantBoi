using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public List<ObjectGenerator> objectGeneratorScripts = new();

    [SerializeField] private GameObject enemyWarning;

    private Transform playerTransform;

    static public float playerDistanceToStandardPos;
    [SerializeField] private float speedIncreaseFactor, loopSpeedMultiplier;
    public float LoopSpeedMultiplier
    {
        get { return loopSpeedMultiplier; }
    }

    private int explosionsGenerated;

    static public bool canGenerateEnemies;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

        FirstGeneration();

        canGenerateEnemies = false;
        playerDistanceToStandardPos = 0;
        ObjectPassingBy.realSpeedMultiplier = 1;
        ObjectPassingBy.speedMultiplier = 1;
    }

    void FirstGeneration()
    {
        float startX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0)).x;
        float finishX = Camera.main.ScreenToWorldPoint(Vector3.zero).x;

        foreach (ObjectGenerator generatorScript in objectGeneratorScripts)
        {
            generatorScript.FirstGeneration(startX, finishX);
        }
    }

    void Update()
    {
        if (Time.deltaTime <= 0) return;

        AdjustSpeedMultiplier();

        foreach (ObjectGenerator generatorScript in objectGeneratorScripts)
        {
            generatorScript.GenerateObject();
        }
    }

    void AdjustSpeedMultiplier()
    {
        if (PlayerController.dead)
        {
            ObjectPassingBy.speedMultiplier /= Time.deltaTime / 4 + 1;
            return;
        }

        ObjectPassingBy.realSpeedMultiplier += Time.deltaTime * speedIncreaseFactor;

        float speedMultiplierFactor = (Mathf.Cos((playerTransform.eulerAngles.z - 180) / 57.3f) + 0.25f) * loopSpeedMultiplier; // For Loops.
        if (speedMultiplierFactor > 1) speedMultiplierFactor = 1;

        ObjectPassingBy.speedMultiplier = ObjectPassingBy.realSpeedMultiplier * speedMultiplierFactor;
    }

    public void ExplosionGenerated()
    {
        explosionsGenerated++;
        if (explosionsGenerated > 0 && !PlayerController.dead)
        {
            canGenerateEnemies = true;
            enemyWarning.SetActive(true);
        }
    }
}
