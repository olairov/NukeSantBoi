using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExplosionController : MonoBehaviour
{
    private Transform playerTramsform, obstaclesContainer, buildingsContainer;

    private SpriteRenderer mySprite;

    private AudioSource concreteSound, glassSound;

    private HudController hudScript;

    private float timeAlive, timeAliveForTimeScale, Acolor = 1, screenshakeValue;

    private int pointsToAdd, comboNum;

    private bool alreadyEnabledHitbox, alreadyAddedPoints, collidedWithPlayer, alreadySentVignetteEffectAnim, cantBreakSkystraperAgain;
    public bool CantBreakSkystraperAgain
    {
        // In case this is the same explosion that broke the original skystraper, it cannot also destroy the new parts.
        set { cantBreakSkystraperAgain = value; }
    }

    void Start()
    {
        if (Time.timeSinceLevelLoad < 0.1f)
        {
            Destroy(gameObject);
        }
        else
        {
            if (GameObject.Find("Player")) playerTramsform = GameObject.Find("Player").transform;
            obstaclesContainer = GameObject.Find("ObstacleGenerator").transform;
            buildingsContainer = GameObject.Find("BuildingGenerator").transform;
            hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();

            mySprite = transform.GetComponent<SpriteRenderer>();

            concreteSound = transform.Find("Sounds/ConcreteSound").GetComponent<AudioSource>();
            glassSound = transform.Find("Sounds/GlassSound").GetComponent<AudioSource>();

            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            transform.parent = GameObject.Find("ExplosionContainer").transform;

            screenshakeValue = PlayerPrefs.GetFloat("ScreenshakeValue");

            Camera.main.GetComponent<ShakeController>().Shake(1);

            GameObject.Find("__________________Map___________________").GetComponent<MapGenerator>().ExplosionGenerated();

            PushObjects();
        }
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;

        if (alreadyEnabledHitbox)
        {
            Acolor -= Time.deltaTime * 1.5f;
            mySprite.color = new Color(mySprite.color.r, mySprite.color.g, mySprite.color.b, Acolor);
        }
        if (timeAlive > 0.1f)
        {
            transform.GetComponent<Collider2D>().enabled = false;

            if (!alreadyAddedPoints && comboNum > 0) AddPoints();
        }

        if (comboNum > 0 && !alreadySentVignetteEffectAnim)
        {
            GameObject.Find("Canvas/VignetteEffect").GetComponent<VignetteEffectController>().Explosion(collidedWithPlayer);
            alreadySentVignetteEffectAnim = true;
        }

        AdjustTimescale();
    }

    void AddPoints()
    {
        if (!PlayerController.dead && pointsToAdd != 0)
        {
            hudScript.ChangePointsValue(pointsToAdd + (comboNum - 1), transform.position + new Vector3(2, 1, 0), comboNum, 0);
        }

        alreadyAddedPoints = true;
    }

    void AdjustTimescale() // Make time warp for an extended epic explosion moment.
    {
        timeAliveForTimeScale += Time.unscaledDeltaTime * 17;

        if (hudScript.actualTimescale < 1) return;

        if (timeAliveForTimeScale >= Mathf.PI * 2) Time.timeScale = 1;
        else
        {
            float multiplier = screenshakeValue / 3; // <--- Increase this number for LESS time warp. Min is 2. Max is infinite (More than 10 won't be noticeable).

            Time.timeScale = Mathf.Cos(timeAliveForTimeScale) * multiplier + 1 - multiplier;
            // FORMULA: y = cos(x) * a + 1 - a
        }
    }

    void PlayDestructAudio(bool isSkyscraper)
    {
        concreteSound.pitch = Random.Range(0.75f, 1.4f);
        concreteSound.Play();

        if (isSkyscraper)
        {
            glassSound.pitch = Random.Range(0.9f, 1.1f);
            glassSound.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(other.transform); // If the object is close, kill it.
        
        if (playerTramsform != null && other.CompareTag("Player") && Vector2.Distance(transform.position, playerTramsform.position) < 2f) collidedWithPlayer = true;
    }

    void Destroy (Transform other)
    {
        if (other.CompareTag("Obstacle"))
        {
            pointsToAdd += 1;
            other.GetComponent<HotAirBalloonScript>().Die();

            comboNum++;
        }
        else if (other.CompareTag("Building") || other.CompareTag("Skyscraper"))
        {
            if (!other.GetComponent<Building>().dead)
            {
                pointsToAdd += 3;
                comboNum++;
            }

            other.GetComponent<Building>().Destruct(transform);

            PlayDestructAudio(!other.CompareTag("Building"));
        }
        else if (other.CompareTag("Enemy"))
        {
            pointsToAdd += 5;
            other.GetComponent<EnemyPlaneController>().Die();

            comboNum++;
        }

        if ((other.CompareTag("Skyscraper") || other.CompareTag("SkyscraperUpperPart")) && !cantBreakSkystraperAgain) // So that the skyscrapers can break multiple times.
        {
            other.GetComponent<SkyscraperBreakAgain>().BreakAgain(transform);
            PlayDestructAudio(true);
        }
    }

    void PushObjects ()
    {
        // Obstacles

        for (int obstacleTypeIdx = 0; obstacleTypeIdx < obstaclesContainer.childCount; obstacleTypeIdx++)
        {
            for (int obstacleIdx = 0; obstacleIdx < obstaclesContainer.childCount; obstacleIdx++)
            {
                Transform obstacleTransform = obstaclesContainer.GetChild(obstacleIdx);
                switch (obstaclesContainer.GetChild(obstacleTypeIdx).name)
                {
                    case "HotAirBalloon":
                        if (Vector2.Distance(obstacleTransform.position, transform.position) > transform.GetComponent<CircleCollider2D>().radius)
                            obstacleTransform.GetComponent<HotAirBalloonScript>().PushAway((obstacleTransform.transform.position - transform.position).normalized, Vector2.Distance(obstacleTransform.position, transform.position));
                        continue;
                    default:
                        continue;
                }
            }
        }

        // Buildings

        for (int buildingClassIdx = 0; buildingClassIdx < buildingsContainer.childCount; buildingClassIdx++)
        {
            for (int buildingIdx = 0; buildingIdx < buildingsContainer.GetChild(buildingClassIdx).childCount; buildingIdx++)
            {
                Transform buildingTransform = buildingsContainer.GetChild(buildingClassIdx).GetChild(buildingIdx);
                if (!buildingTransform.CompareTag("Building")) continue;
                buildingTransform.GetComponent<Building>().PushAway((buildingTransform.transform.position - transform.position).normalized.x, Vector2.Distance(buildingTransform.position, transform.position));
            }
        }
    }
}
