using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //Background objects prefabs:
    [SerializeField] private GameObject layer1Background, layer2Background, layer3Background;
    public Sprite lastBackgroundLayer1, lastBackgroundLayer2, lastBackgroundLayer3, lastBuildingSprite, lastWideBuildingSprite;

    //Interactable objects prefabs:
    [SerializeField] private GameObject buildingPrefab, wideBuildingPrefab, enemyPrefab, skystraperPrefab, obstaclePrefab, frontBirdPrefab, bushPrefab;

    //Other Prefabs:
    [SerializeField] private GameObject warningPrefab, birdGroupPrefab, singleBirdPrefab, cranePrefab;
    [SerializeField] private GameObject enemyWarning;

    private Transform playerTransform ,buildingsContainer, enemiesContainer, obstaclesContainer, backgroundContainer, warningsContainerCanvas,
        detailsContainer;

    static public float playerDistanceToStandardPos;
    [SerializeField] private float speedIncreaseFactor, chanceItIsWideBuilding, loopSpeedMultiplier;
    public float LoopSpeedMultiplier
    {
        get { return loopSpeedMultiplier; }
    }

    // Times for every thing generation
    private float timeForNextBuilding, timeForNextEnemy = 3, timeForNextObstacle, timeForNextLayer1, timeForNextLayer2,
        timeForNextLayer3, timeForNextBirdGroup = 3, timeForSingleBird = 1, timeForNextCrane = 8, timeForNextFrontBird, timeForNextBush = 1;

    private int buildingsFromSkystraper, explosionsGenerated;

    static private bool firstGamesBuildingGenerated;
    private bool canGenerateEnemies, firstBuildingGenerated;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        buildingsContainer = GameObject.Find("BuildingsContainer").transform;
        enemiesContainer = GameObject.Find("EnemiesContainer").transform;
        obstaclesContainer = GameObject.Find("ObstaclesContainer").transform;
        backgroundContainer = GameObject.Find("BackgroundContainer").transform;
        warningsContainerCanvas = GameObject.Find("Canvas/Warning").transform;
        detailsContainer = GameObject.Find("DetailsContainer").transform;

        FirstGeneration();

        playerDistanceToStandardPos = 0;
        ObjectPassingBy.realSpeedMultiplier = 1;
        ObjectPassingBy.speedMultiplier = 1;

        timeForNextFrontBird = Random.Range(10, 60);
    }

    void Update()
    {
        if (Time.deltaTime <= 0) return;

        if (!PlayerController.dead)
        {
            ObjectPassingBy.realSpeedMultiplier += Time.deltaTime * speedIncreaseFactor;

            float speedMultiplierFactor = (Mathf.Cos((playerTransform.eulerAngles.z - 180) / 57.3f) + 0.25f) * loopSpeedMultiplier; // For Loops.
            if (speedMultiplierFactor > 1) speedMultiplierFactor = 1;

            ObjectPassingBy.speedMultiplier = ObjectPassingBy.realSpeedMultiplier * speedMultiplierFactor;
        }
        else ObjectPassingBy.speedMultiplier /= Time.deltaTime / 4 + 1;

        GenerateBuildings();
        GenerateObstacles();
        GenerateBirdGroups();
        GenerateSingleBirds();
        GenerateCranes();
        GenerateFrontBirds();
        GenerateBushes();

        if (!PlayerController.dead && canGenerateEnemies) GenerateEnemies();

        Layer1BackgroundGeneration();
        Layer2BackgroundGeneration();
        Layer3BackgroundGeneration();

        //ObjectPassingBy.speedMultiplier = 0;
        //ObjectPassingBy.realSpeedMultiplier = 0;
    }

    void GenerateBuildings()
    {
        timeForNextBuilding -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextBuilding > 0) return;
        
        if (!firstGamesBuildingGenerated)
        {
            firstGamesBuildingGenerated = true;
            firstBuildingGenerated = true;
            timeForNextBuilding = 1;
            return; // For some reason, the first building of the first game overlaps with another one, so i had to do this against my will.
        }

        if (Random.value * ObjectPassingBy.realSpeedMultiplier < 0.15f && buildingsFromSkystraper > 3 * ObjectPassingBy.realSpeedMultiplier)
        {
            Instantiate(skystraperPrefab, buildingsContainer);
            buildingsFromSkystraper = 0;

            if (!PlayerController.dead) Instantiate(warningPrefab, warningsContainerCanvas);
        }
        else
        {
            if (Random.value > chanceItIsWideBuilding)
            {
                if (lastBuildingSprite == null) Instantiate(buildingPrefab, buildingsContainer);
                else Instantiate(buildingPrefab, buildingsContainer).GetComponent<Building>().lastSprite = lastBuildingSprite;
            }
            else
            {
                if (lastBuildingSprite == null) Instantiate(wideBuildingPrefab, buildingsContainer);
                else Instantiate(wideBuildingPrefab, buildingsContainer).GetComponent<Building>().lastSprite = lastWideBuildingSprite;
            }
        }
        
        if (firstBuildingGenerated) timeForNextBuilding = Random.Range(1.6f, 2.2f);
        else
        {
            firstBuildingGenerated = true;
            timeForNextBuilding = 2.8f;
            // For any other reason, the first building to be generated at the beginning appears too close to the next one. Now it doesn't.
        }

        timeForNextBuilding = Random.Range(1.6f, 2.2f);
        buildingsFromSkystraper++;
    }

    void GenerateEnemies()
    {
        timeForNextEnemy -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextEnemy > 0) return;

        Instantiate(enemyPrefab, enemiesContainer);

        timeForNextEnemy = Random.Range(4.5f, 6f);
    }

    void GenerateObstacles()
    {
        timeForNextObstacle -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextObstacle > 0) return;

        Instantiate(obstaclePrefab, obstaclesContainer);

        timeForNextObstacle = Random.Range(3.2f, 5f);
    }

    void GenerateBirdGroups()
    {
        timeForNextBirdGroup -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextBirdGroup > 0) return;

        Instantiate(birdGroupPrefab, detailsContainer);

        timeForNextBirdGroup = Random.Range(12f, 20f);
    }

    void GenerateSingleBirds()
    {
        timeForSingleBird -= Time.deltaTime;
        if (timeForSingleBird > 0) return;

        Instantiate(singleBirdPrefab, detailsContainer);

        timeForSingleBird = Random.Range(8f, 14f);
    }

    void GenerateCranes()
    {
        timeForNextCrane -= Time.deltaTime;
        if (timeForNextCrane > 0) return;

        Instantiate(cranePrefab, detailsContainer);

        timeForNextCrane = Random.Range(8f, 24f);
    }

    void GenerateFrontBirds()
    {
        timeForNextFrontBird -= Time.deltaTime;
        if (timeForNextFrontBird > 0) return;

        Instantiate(frontBirdPrefab, new Vector3(0, Random.Range(6f, -6f), -10), Quaternion.identity, detailsContainer);

        timeForNextFrontBird = Random.Range(18, 60);
    }

    void GenerateBushes()
    {
        timeForNextBush -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextBush > 0) return;

        Instantiate(bushPrefab, detailsContainer);

        timeForNextBush = Random.Range(1.2f, 2.4f);
    }

    void Layer1BackgroundGeneration()
    {
        timeForNextLayer1 -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextLayer1 > 0) return;

        Instantiate(layer1Background, backgroundContainer).GetComponent<BackgroundController>().LastBackground = lastBackgroundLayer1;

        timeForNextLayer1 = 5f;
    }

    void Layer2BackgroundGeneration()
    {
        timeForNextLayer2 -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextLayer2 > 0) return;

        Instantiate(layer2Background, backgroundContainer).GetComponent<BackgroundController>().LastBackground = lastBackgroundLayer2;

        timeForNextLayer2 = 7f;
    }

    void Layer3BackgroundGeneration()
    {
        timeForNextLayer3 -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextLayer3 > 0) return;

        Instantiate(layer3Background, backgroundContainer).GetComponent<BackgroundController>().LastBackground = lastBackgroundLayer3;

        timeForNextLayer3 = 8f;
    }

    void FirstGeneration()
    {
        float startX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0)).x;
        float finishX = Camera.main.ScreenToWorldPoint(Vector3.zero).x;

        Vector3 buildingDefaultPos = buildingPrefab.transform.position;

        for (float actualX = startX; actualX > finishX; actualX -= 8.5f)
        {
            if (Random.value < 0.7f) Instantiate(buildingPrefab, new Vector3(actualX, buildingDefaultPos.y, buildingDefaultPos.z), Quaternion.identity, buildingsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            else Instantiate(wideBuildingPrefab, new Vector2(actualX, buildingDefaultPos.y), Quaternion.identity, buildingsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        }

        for (float actualX = startX; actualX > finishX; actualX -= Random.Range(6, 10))
        {
            Instantiate(bushPrefab, new Vector3(actualX, -6, 0), Quaternion.identity, detailsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        }

        for (float actualX = startX + 5; actualX > finishX - 30; actualX -= 24.5f)
        {
            Instantiate(layer1Background, new Vector2(actualX, 0), Quaternion.identity, backgroundContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            Instantiate(layer2Background, new Vector2(actualX, 0), Quaternion.identity, backgroundContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            Instantiate(layer3Background, new Vector2(actualX, 0), Quaternion.identity, backgroundContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        }

        Instantiate(birdGroupPrefab, new Vector3(Random.Range(startX - 2, finishX + 5), 0, birdGroupPrefab.transform.position.z), Quaternion.identity, detailsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        Instantiate(singleBirdPrefab, new Vector3(Random.Range(startX - 2, finishX + 5), 0, birdGroupPrefab.transform.position.z), Quaternion.identity, detailsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        Instantiate(cranePrefab, new Vector3(Random.Range(startX - 2, finishX + 5), 0, cranePrefab.transform.position.z), Quaternion.identity, detailsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
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
