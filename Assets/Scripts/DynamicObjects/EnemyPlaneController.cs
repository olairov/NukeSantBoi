using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneController : MonoBehaviour
{
    [SerializeField] private Color burnColor, possibleColor1, possibleColor2, possibleColor3;

    [SerializeField] private float moveSpeed, rotSpeed;
    public float rotationSpeed;

    public bool dead, dutyFinished;

    private Vector3 dirToPlayer;

    private Transform playerTransform;

    private Rigidbody2D rb;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        rb = transform.GetComponent<Rigidbody2D>();

        ChoseColor();

        transform.position = new Vector3(transform.position.x, Random.Range(Camera.main.ScreenToWorldPoint(Vector3.zero).y, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y), transform.position.z);
    }

    void ChoseColor()
    {
        Color chosenColor = possibleColor1;
        float randValue = Random.value;
        if (randValue > 0.66f) chosenColor = possibleColor2;
        else if (randValue > 0.33f) chosenColor = possibleColor3;

        for (int childNum = 0; childNum < transform.Find("Parts").childCount; childNum++) transform.Find("Parts").GetChild(childNum).GetComponent<SpriteRenderer>().color = chosenColor;
    }

    void Update()
    {
        if (!dead) RotateAndMove();

        if (transform.position.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y - 1 || transform.position.y > Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y + 1) Destroy(gameObject);
    }

    void RotateAndMove()
    {
        float lastRotation = transform.eulerAngles.z;
        if (!PlayerController.dead) dirToPlayer = playerTransform.position - transform.position;

        if (!dutyFinished)
        {
            if (dirToPlayer.x > 1 || PlayerController.dead)
            {
                dutyFinished = true;
                for (int childNum = 0; childNum < transform.Find("Parts").childCount - 1; childNum++) transform.Find("Parts").GetChild(childNum + 1).GetComponent<PlayerPartParallaxer>().enabled = false;
            }
            else if ((dirToPlayer.y < 0 && transform.eulerAngles.z < 145) || (dirToPlayer.y > 0 && transform.eulerAngles.z > 45))
            {
                Vector3 rotationAdding = dirToPlayer * Time.deltaTime * rotSpeed;
                transform.up += rotationAdding;
            }
        }/*
        else
        {
            Vector3 rotationAdding = new Vector3(-Mathf.Lerp(dirToPlayer.x, -1, Mathf.Clamp01((Time.time - timeSinceFinished) / 50)), -Mathf.Lerp(dirToPlayer.y, 0, Mathf.Clamp01((Time.time - timeSinceFinished) / 50)), -Mathf.Lerp(dirToPlayer.z, 0, Mathf.Clamp01((Time.time - timeSinceFinished) / 50))) * Time.deltaTime * rotSpeed;
            transform.up += rotationAdding;
        }*/

        Vector3 forceToAdd = new Vector3(0, -(transform.eulerAngles.z - 90) / 90 * moveSpeed, 0) * Time.deltaTime;
        transform.position += forceToAdd;

        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
        rotationSpeed = (transform.eulerAngles.z - lastRotation) / Time.unscaledDeltaTime;
    }

    public void Die()
    {
        if (dead) return;
        dead = true;

        rb.angularDrag = 0;
        rb.gravityScale = 2;

        rb.AddTorque(Random.Range(80f, 240f));
        if (Random.value > 0.5f) rb.angularVelocity *= -1;
        rb.AddForce(new Vector2(Random.Range(-2, 3), 5), ForceMode2D.Impulse);

        for (int childNum = 0; childNum < transform.Find("Parts").childCount; childNum++) transform.Find("Parts").GetChild(childNum).GetComponent<SpriteRenderer>().color = burnColor;

        transform.GetComponent<Collider2D>().enabled = false;
        transform.GetComponent<ObjectPassingBy>().enabled = false;
    }
}
