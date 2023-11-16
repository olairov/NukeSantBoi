using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] private GameObject pointsPrefab;
    private Transform pointsContainer;

    private SpriteRenderer mySprite;

    private HudController hudScript;

    private float timeStart, Acolor = 1;

    private int pointsToAdd;

    private bool alreadyEnabledHitbox, alreadyAddedPoints;

    void Start()
    {
        if (Time.timeSinceLevelLoad < 0.1f) Destroy(gameObject);
        
        pointsContainer = GameObject.Find("NotPhysicElementsContainer").transform;
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();

        mySprite = transform.GetComponent<SpriteRenderer>();
        timeStart = Time.time;

        transform.position = new Vector3(transform.position.x, transform.position.y, -2);
        transform.parent = GameObject.Find("ExplosionContainer").transform;

        Camera.main.GetComponent<ShakeController>().Shake();
    }

    private void Update()
    {
        if (Time.time - timeStart > 0.08f && !alreadyEnabledHitbox) EnableHitbox();
        if (alreadyEnabledHitbox)
        {
            Acolor -= Time.deltaTime * 1.5f;
            mySprite.color = new Color(mySprite.color.r, mySprite.color.g, mySprite.color.b, Acolor);
        }
        if (Time.time - timeStart > 0.1f && alreadyEnabledHitbox && !alreadyAddedPoints)
        {
            if (!PlayerController.dead && pointsToAdd != 0)
            {
                Instantiate(pointsPrefab, transform.position + new Vector3(2, 1, -1), Quaternion.identity, pointsContainer).GetComponentInChildren<TMP_Text>().text = "+" + pointsToAdd;
                hudScript.ChangePointsValue(pointsToAdd);
            }

            transform.GetComponent<Collider2D>().enabled = false;

            alreadyAddedPoints = true;
        }
    }

    void EnableHitbox()
    {
        transform.GetComponent<Collider2D>().enabled = true;

        alreadyEnabledHitbox = true;
    }

    void PlayDestructAudio()
    {
        AudioSource myAudio = transform.GetComponent<AudioSource>();

        myAudio.pitch = Random.Range(0.75f, 1.4f);
        myAudio.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            pointsToAdd += 1;
            other.GetComponent<ObstacleScript>().Die();
        }
        if (other.CompareTag("Building") || other.CompareTag("Skystraper")) if (!other.GetComponent<Building>().dead)
        {
            pointsToAdd += 3;
            other.GetComponent<Building>().Destruct(transform.position.y);

            PlayDestructAudio();
        }
        if (other.CompareTag("Enemy"))
        {
            pointsToAdd += 5;
            other.GetComponent<EnemyPlaneController>().Die();
        }
    }
}
