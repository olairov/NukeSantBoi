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

    private float timeAlive, timeAliveForTimeScale, Acolor = 1;

    private int pointsToAdd, comboNum;

    private bool alreadyEnabledHitbox, alreadyAddedPoints, collidedWithPlayer, alreadySentVignetteEffectAnim;

    void Start()
    {
        if (Time.timeSinceLevelLoad < 0.1f) Destroy(gameObject);
        
        pointsContainer = GameObject.Find("NotPhysicElementsContainer").transform;
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();

        mySprite = transform.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(transform.position.x, transform.position.y, -25);
        transform.parent = GameObject.Find("ExplosionContainer").transform;

        Camera.main.GetComponent<ShakeController>().Shake();
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;

        if (timeAlive > 0.08f && !alreadyEnabledHitbox) EnableHitbox();
        if (alreadyEnabledHitbox)
        {
            Acolor -= Time.deltaTime * 1.5f;
            mySprite.color = new Color(mySprite.color.r, mySprite.color.g, mySprite.color.b, Acolor);
        }
        if (timeAlive > 0.1f && !alreadyAddedPoints && comboNum > 0)
        {
            if (!PlayerController.dead && pointsToAdd != 0)
            {
                if (comboNum > 1)
                {
                    TMP_Text comboText = Instantiate(pointsPrefab, transform.position + new Vector3(2, 2.5f, -1), Quaternion.identity, pointsContainer).GetComponentInChildren<TMP_Text>();
                    comboText.fontSize /= 2;
                    comboText.text = "COMBO x" + comboNum;
                }
                Instantiate(pointsPrefab, transform.position + new Vector3(2, 1, -1), Quaternion.identity, pointsContainer).GetComponentInChildren<TMP_Text>().text = "+" + (pointsToAdd + (comboNum - 1));

                hudScript.ChangePointsValue(pointsToAdd + (comboNum - 1));
            }

            transform.GetComponent<Collider2D>().enabled = false;

            alreadyAddedPoints = true;
        }

        if (comboNum > 0 && !alreadySentVignetteEffectAnim)
        {
            GameObject.Find("Camera/CameraRiser/Main Camera/VignetteEffect").GetComponent<VignetteEffectController>().Explosion(collidedWithPlayer);
            alreadySentVignetteEffectAnim = true;
        }

        AdjustTimescale();
    }

    void AdjustTimescale()
    {
        timeAliveForTimeScale += Time.unscaledDeltaTime * 17;

        if (hudScript.actualTimescale < 1) return;

        if (timeAliveForTimeScale >= Mathf.PI * 2) Time.timeScale = 1;
        else
        {
            Time.timeScale = Mathf.Cos(timeAliveForTimeScale) / 4 + 0.75f;
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

            comboNum++;
        }
        if (other.CompareTag("Building") || other.CompareTag("Skystraper"))
        {
            if (!other.GetComponent<Building>().dead) pointsToAdd += 3;
            other.GetComponent<Building>().Destruct(transform.position);

            PlayDestructAudio();

            comboNum++;
        }
        if (other.CompareTag("Enemy"))
        {
            pointsToAdd += 5;
            other.GetComponent<EnemyPlaneController>().Die();

            comboNum++;
        }

        if (other.CompareTag("Player")) collidedWithPlayer = true;
    }
}
