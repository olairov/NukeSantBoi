using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] private GameObject pointsPrefab;
    private Transform pointsContainer;

    private SpriteRenderer mySprite;

    private float timeStart, Acolor = 1;

    private int pointsToAdd;

    private bool shortExplosion, alreadyGeneratedPoints;

    void Start()
    {
        pointsContainer = GameObject.Find("NotPhysicElementsContainer").transform;

        mySprite = transform.GetComponent<SpriteRenderer>();
        timeStart = Time.time;

        transform.position = new Vector3(transform.position.x, transform.position.y, -2);
        transform.parent = GameObject.Find("ExplosionContainer").transform;
    }

    private void Update()
    {
        if (Time.time - timeStart > 0.05f)
        {
            transform.GetComponent<Collider2D>().enabled = true;

            Acolor -= Time.deltaTime * 1.5f;
            mySprite.color = new Color(mySprite.color.r, mySprite.color.g, mySprite.color.b, Acolor);

            if (!PlayerController.dead && pointsToAdd != 0 && !alreadyGeneratedPoints)
            {
                Instantiate(pointsPrefab, transform.position + new Vector3(2, 1, -1), Quaternion.identity, pointsContainer).GetComponent<TMP_Text>().text = "+" + pointsToAdd;
                alreadyGeneratedPoints = true;
            }
        }
        if (Time.time - timeStart > 0.45f) transform.GetComponent<Collider2D>().enabled = false;
        if (shortExplosion && Time.time - timeStart > 0.2f) transform.GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Skystraper") shortExplosion = true;

        if (alreadyGeneratedPoints) pointsToAdd = 0;
        if (other.name.StartsWith("Obstacle")) pointsToAdd += 1;
        else if (other.name.StartsWith("Building")) pointsToAdd += 3;
        else if (other.name.StartsWith("Enemy")) pointsToAdd += 5;

        if (alreadyGeneratedPoints && pointsToAdd != 0) Instantiate(pointsPrefab, transform.position + new Vector3(0, 1), Quaternion.identity, pointsContainer).GetComponent<TMP_Text>().text = "+" + pointsToAdd;
    }
}
