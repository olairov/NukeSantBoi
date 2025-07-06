using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Each Generator Script
    public BuildingGenerator buildingGeneratorScript;
    public BackgroundGenerator backgroundGeneratorScript;
    public ObstacleGenerator obstacleGeneratorScript;
    public EnemyGenerator enemyGeneratorScript;
    public DetailGenerator detailGeneratorScript;

    [SerializeField] private GameObject enemyWarning;

    private Transform playerTransform;

    static public float playerDistanceToStandardPos;
    [SerializeField] private float speedIncreaseFactor, loopSpeedMultiplier;
    public float LoopSpeedMultiplier
    {
        get { return loopSpeedMultiplier; }
    }

    private int explosionsGenerated;

    private bool canGenerateEnemies;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

        FirstGeneration();

        playerDistanceToStandardPos = 0;
        ObjectPassingBy.realSpeedMultiplier = 1;
        ObjectPassingBy.speedMultiplier = 1;
    }

    void Update()
    {
        if (Time.deltaTime <= 0) return;

        AdjustSpeedMultiplier();

        buildingGeneratorScript.GenerateBuilding();
        backgroundGeneratorScript.GenerateBackground();
        obstacleGeneratorScript.GenerateObstacles();
        if (!PlayerController.dead && canGenerateEnemies) enemyGeneratorScript.GenerateEnemies();
        detailGeneratorScript.GenerateDetails();
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

    void FirstGeneration()
    {
        float startX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0)).x;
        float finishX = Camera.main.ScreenToWorldPoint(Vector3.zero).x;

        buildingGeneratorScript.FirstGeneration(startX, finishX);
        backgroundGeneratorScript.FirstGeneration(startX, finishX);
        obstacleGeneratorScript.FirstGeneration(startX, finishX);
        enemyGeneratorScript.FirstGeneration(startX, finishX);
        detailGeneratorScript.FirstGeneration(startX, finishX);
    }

    public void ExplosionGenerated()
    {
        explosionsGenerated++;
        if (explosionsGenerated > 2 && !PlayerController.dead)
        {
            canGenerateEnemies = true;
            enemyWarning.SetActive(true);
        }
    }
}
