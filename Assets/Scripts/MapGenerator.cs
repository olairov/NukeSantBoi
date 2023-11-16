using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //Background objects prefabs:
    [SerializeField] private GameObject layer1Background, layer2Background, layer3Background;
    public Sprite lastBackgroundLayer1, lastBackgroundLayer2, lastBackgroundLayer3;

    //Interactable objects prefabs:
    [SerializeField] private GameObject buildingPrefab, wideBuildingPrefab, enemyPrefab, skystraperPrefab, obstaclePrefab, warningPrefab;

    private Transform buildingsContainer, enemiesContainer, obstaclesContainer, backgroundContainer, warningsContainer;

    static public float playerDistanceToObjective;
    [SerializeField] private float speedIncreaseFactor;
    private float timeForNextBuilding, timeForNextEnemy = 6, timeForNextObstacle = 2, timeForNextLayer1, timeForNextLayer2, timeForNextLayer3;

    private int buildingsFromSkystraper;

    private bool isPaused;
    public bool SetIsPaused
    {
        set { isPaused = value; }
    }

    void Start()
    {
        buildingsContainer = GameObject.Find("BuildingsContainer").transform;
        enemiesContainer = GameObject.Find("EnemiesContainer").transform;
        obstaclesContainer = GameObject.Find("ObstaclesContainer").transform;
        backgroundContainer = GameObject.Find("BackgroundContainer").transform;
        warningsContainer = GameObject.Find("NotPhysicElementsContainer").transform;

        FirstGeneration();

        playerDistanceToObjective = 0;
        ObjectPassingBy.speedMultiplyer = 1;
    }

    void Update()
    {
        if (isPaused) return;
        
        if (!PlayerController.dead) ObjectPassingBy.speedMultiplyer += Time.deltaTime * speedIncreaseFactor;
        else ObjectPassingBy.speedMultiplyer /= Time.deltaTime / 4 + 1;

        GenerateBuildings();
        GenerateObstacles();
        if(!PlayerController.dead) GenerateEnemies();

        Layer1BackgroundGeneration();
        Layer2BackgroundGeneration();
        Layer3BackgroundGeneration();
    }

    void GenerateBuildings()
    {
        timeForNextBuilding -= Time.deltaTime;
        if (timeForNextBuilding > 0) return;

        if (Random.value * ObjectPassingBy.speedMultiplyer < 0.15f && buildingsFromSkystraper > 3 * ObjectPassingBy.speedMultiplyer)
        {
            Instantiate(skystraperPrefab, buildingsContainer);
            buildingsFromSkystraper = 0;

            if (timeForNextEnemy < 1.2f) GenerateEnemies();
            timeForNextEnemy = 4;

            if (!PlayerController.dead) Instantiate(warningPrefab, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.75f, Screen.height * 0.75f, 7)), Quaternion.Euler(0, 0, 180), warningsContainer);
        }
        else
        {
            if (Random.value < 0.7f) Instantiate(buildingPrefab, buildingsContainer);
            else Instantiate(wideBuildingPrefab, buildingsContainer);
        }

        timeForNextBuilding = Random.Range(1.2f, 2.4f) / ObjectPassingBy.speedMultiplyer;

        buildingsFromSkystraper++;
    }

    void GenerateEnemies()
    {
        timeForNextEnemy -= Time.deltaTime;
        if (timeForNextEnemy > 0) return;

        Instantiate(enemyPrefab, enemiesContainer);

        timeForNextEnemy = Random.Range(1.6f, 3.6f);
    }

    void GenerateObstacles()
    {
        timeForNextObstacle -= Time.deltaTime;
        if (timeForNextObstacle > 0) return;

        Instantiate(obstaclePrefab, obstaclesContainer);

        timeForNextObstacle = Random.Range(1.2f, 4.8f);
    }

    void Layer1BackgroundGeneration()
    {
        timeForNextLayer1 -= Time.deltaTime;
        if (timeForNextLayer1 > 0) return;

        Instantiate(layer1Background, backgroundContainer).GetComponent<BackgroundController>().LastBackground = lastBackgroundLayer1;

        timeForNextLayer1 = 5f / ObjectPassingBy.speedMultiplyer;
    }

    void Layer2BackgroundGeneration()
    {
        timeForNextLayer2 -= Time.deltaTime;
        if (timeForNextLayer2 > 0) return;

        Instantiate(layer2Background, backgroundContainer).GetComponent<BackgroundController>().LastBackground = lastBackgroundLayer2;

        timeForNextLayer2 = 7f / ObjectPassingBy.speedMultiplyer;
    }

    void Layer3BackgroundGeneration()
    {
        timeForNextLayer3 -= Time.deltaTime;
        if (timeForNextLayer3 > 0) return;

        Instantiate(layer3Background, backgroundContainer).GetComponent<BackgroundController>().LastBackground = lastBackgroundLayer3;

        timeForNextLayer3 = 8f / ObjectPassingBy.speedMultiplyer;
    }

    void FirstGeneration()
    {
        float startX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0)).x;
        float finishX = Camera.main.ScreenToWorldPoint(Vector3.zero).x;

        for (float actualX = startX; actualX > finishX; actualX -= 7.5f)
        {
            if (Random.value < 0.7f) Instantiate(buildingPrefab, new Vector3(actualX, 0, 0), Quaternion.identity, buildingsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            else Instantiate(wideBuildingPrefab, new Vector2(actualX, 0), Quaternion.identity, buildingsContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        }

        for (float actualX = startX + 5; actualX > finishX - 30; actualX -= 24.5f)
        {
            Instantiate(layer1Background, new Vector3(actualX, 0, 1), Quaternion.identity, backgroundContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            Instantiate(layer2Background, new Vector3(actualX, 0, 2), Quaternion.identity, backgroundContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
            Instantiate(layer3Background, new Vector3(actualX, 0, 3), Quaternion.identity, backgroundContainer).GetComponent<ObjectPassingBy>().appearingObject = true;
        }
    }
}
