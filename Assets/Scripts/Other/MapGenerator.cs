using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //Background objects prefabs:
    [SerializeField] private GameObject layer1Background, layer2Background, layer3Background;
    public Sprite lastBackgroundLayer1, lastBackgroundLayer2, lastBackgroundLayer3, lastBuildingSprite, lastWideBuildingSprite;

    //Interactable objects prefabs:
    [SerializeField] private GameObject buildingPrefab, wideBuildingPrefab, enemyPrefab, skystraperPrefab, obstaclePrefab, warningPrefab, birdGroupPrefab, singleBirdPrefab;

    //Other Prefabs:
    [SerializeField] private GameObject wind1pref, wind2pref, wind3pref, wind4pref;

    private Transform playerTransform ,buildingsContainer, enemiesContainer, obstaclesContainer, backgroundContainer, warningsContainer, particlesContainer, detailsContainer;

    static public float playerDistanceToStandardPos;
    [SerializeField] private float speedIncreaseFactor;
    private float timeForNextBuilding, timeForNextEnemy, timeForNextObstacle, timeForNextParticle, timeForNextLayer1, timeForNextLayer2, timeForNextLayer3, timeForNextBirdGroup = 3, timeForSingleBird = 1;

    private int buildingsFromSkystraper;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        buildingsContainer = GameObject.Find("BuildingsContainer").transform;
        enemiesContainer = GameObject.Find("EnemiesContainer").transform;
        obstaclesContainer = GameObject.Find("ObstaclesContainer").transform;
        backgroundContainer = GameObject.Find("BackgroundContainer").transform;
        warningsContainer = GameObject.Find("NotPhysicElementsContainer").transform;
        particlesContainer = GameObject.Find("ParticlesContainer").transform;
        detailsContainer = GameObject.Find("DetailsContainer").transform;

        FirstGeneration();

        playerDistanceToStandardPos = 0;
        ObjectPassingBy.realSpeedMultiplier = 1;
        ObjectPassingBy.speedMultiplier = 1;
    }

    void Update()
    {
        if (Time.deltaTime <= 0) return;
        
        if (!PlayerController.dead)
        {
            ObjectPassingBy.realSpeedMultiplier += Time.deltaTime * speedIncreaseFactor;

            float fallSpeedMultiplier = Mathf.Cos((playerTransform.eulerAngles.z - 122) / 57.3f) * 1.6f - 0.5f; // For slightly increase speed when going down.
            if (fallSpeedMultiplier < 1) fallSpeedMultiplier = 1;
            float speedMultiplierFactor = (Mathf.Cos((playerTransform.eulerAngles.z - 180) / 57.3f) * 1.2f + 0.5f); // For Loops.
            if (speedMultiplierFactor > 1) speedMultiplierFactor = 1;

            ObjectPassingBy.speedMultiplier = ObjectPassingBy.realSpeedMultiplier * speedMultiplierFactor * fallSpeedMultiplier;
        }
        else ObjectPassingBy.speedMultiplier /= Time.deltaTime / 4 + 1;

        GenerateBuildings();
        GenerateObstacles();
        GenerateWindParticles();
        GenerateBirdGroups();
        GenerateSingleBirds();

        if(!PlayerController.dead) GenerateEnemies();

        Layer1BackgroundGeneration();
        Layer2BackgroundGeneration();
        Layer3BackgroundGeneration();
    }

    void GenerateBuildings()
    {
        timeForNextBuilding -= Time.unscaledDeltaTime * Time.timeScale * ObjectPassingBy.speedMultiplier;
        if (timeForNextBuilding > 0) return;

        if (Random.value * ObjectPassingBy.realSpeedMultiplier < 0.15f && buildingsFromSkystraper > 3 * ObjectPassingBy.realSpeedMultiplier)
        {
            Instantiate(skystraperPrefab, buildingsContainer);
            buildingsFromSkystraper = 0;

            if (timeForNextEnemy < 1.2f) GenerateEnemies();
            timeForNextEnemy = 4;

            if (!PlayerController.dead) Instantiate(warningPrefab, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.75f, Screen.height * 0.75f, 7)), Quaternion.Euler(0, 0, 180), warningsContainer);
        }
        else
        {
            if (Random.value < 0.7f)
            {
                if (lastBuildingSprite == null) Instantiate(buildingPrefab, buildingsContainer);
                else Instantiate(buildingPrefab, buildingsContainer).GetComponent<Building>().lastSprite = lastBuildingSprite;
            }
            else
            {
                if (lastBuildingSprite == null) Instantiate(buildingPrefab, buildingsContainer);
                else Instantiate(wideBuildingPrefab, buildingsContainer).GetComponent<Building>().lastSprite = lastWideBuildingSprite;
            }
        }

        timeForNextBuilding = Random.Range(1.6f, 2.2f);

        buildingsFromSkystraper++;
    }

    void GenerateEnemies()
    {
        timeForNextEnemy -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextEnemy > 0) return;

        Instantiate(enemyPrefab, enemiesContainer);

        timeForNextEnemy = Random.Range(1.6f, 3.6f);
    }

    void GenerateObstacles()
    {
        timeForNextObstacle -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextObstacle > 0) return;

        Instantiate(obstaclePrefab, obstaclesContainer);

        timeForNextObstacle = Random.Range(3.2f, 5f);
    }

    void GenerateWindParticles()
    {
        timeForNextParticle -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextParticle > 0) return;

        float randValue = Random.value;

        if (randValue > 0.75f) Instantiate(wind1pref, particlesContainer);
        else if (randValue > 0.5f) Instantiate(wind2pref, particlesContainer);
        else if (randValue > 0.25f) Instantiate(wind3pref, particlesContainer);
        else Instantiate(wind4pref, particlesContainer);

        timeForNextParticle = Random.Range(0.4f, 0.6f);
    }

    void GenerateBirdGroups()
    {
        timeForNextBirdGroup -= Time.deltaTime * ObjectPassingBy.speedMultiplier;
        if (timeForNextBirdGroup > 0) return;

        Instantiate(birdGroupPrefab, detailsContainer);

        timeForNextBirdGroup = Random.Range(8f, 16f);
    }

    void GenerateSingleBirds()
    {
        timeForSingleBird -= Time.deltaTime;
        if (timeForSingleBird > 0) return;

        Instantiate(singleBirdPrefab, detailsContainer);

        timeForSingleBird = Random.Range(6f, 12f);
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

        for (float actualX = startX; actualX > finishX; actualX -= 7.5f)
        {
            if (Random.value < 0.7f) Instantiate(buildingPrefab, new Vector3(actualX, buildingDefaultPos.y, buildingDefaultPos.z), Quaternion.identity, buildingsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            else Instantiate(wideBuildingPrefab, new Vector2(actualX, buildingDefaultPos.y), Quaternion.identity, buildingsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        }

        for (float actualX = startX + 5; actualX > finishX - 30; actualX -= 24.5f)
        {
            Instantiate(layer1Background, new Vector2(actualX, 0), Quaternion.identity, backgroundContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            Instantiate(layer2Background, new Vector2(actualX, 0), Quaternion.identity, backgroundContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            Instantiate(layer3Background, new Vector2(actualX, 0), Quaternion.identity, backgroundContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        }

        Instantiate(birdGroupPrefab, new Vector3(Random.Range(startX - 2, finishX + 5), 0, birdGroupPrefab.transform.position.z), Quaternion.identity, detailsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        Instantiate(singleBirdPrefab, new Vector3(Random.Range(startX - 2, finishX + 5), 0, birdGroupPrefab.transform.position.z), Quaternion.identity, detailsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
    }
}
