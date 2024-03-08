using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneController : MonoBehaviour
{
    [SerializeField] private Color burnColor, possibleColor1, possibleColor2, possibleColor3;

    [SerializeField] private float moveSpeed, rotFactor;
    public float actualRotationSpeed;
    private float maxXdistance;

    public bool dead, dutyFinished;

    private Vector3 dirToPlayer;

    private Transform playerTransform;

    private Rigidbody2D rb;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        rb = transform.GetComponent<Rigidbody2D>();

        ChoseColor();

        maxXdistance = transform.position.x - Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 5, 0)).x;
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
        actualRotationSpeed = transform.eulerAngles.z;

        if (!dutyFinished)
        {
            TowardsPlayer();
            if (dirToPlayer.x > 1 || PlayerController.dead) dutyFinished = true;
        }
        else AfterPlayer();

        actualRotationSpeed -= transform.eulerAngles.z;

        transform.position += new Vector3(0, -(transform.eulerAngles.z - 90) * moveSpeed, 0);
    }

    void TowardsPlayer()
    {
        Vector2 distToPlayer = new Vector2(transform.position.x - playerTransform.position.x, transform.position.y - playerTransform.position.y);

        transform.eulerAngles = new Vector3(0, 0, 90 + (rotFactor * distToPlayer.y * (-distToPlayer.x / maxXdistance + 1)));
    }

    void AfterPlayer()
    {

    }

    // Old Way of rotating and moving. Need to create a better one.

    /*void RotateAndMove()
    {
        float lastRotation = transform.eulerAngles.z;
        if (!PlayerController.dead) dirToPlayer = playerTransform.position - transform.position;

        Vector3 forceToAdd = new Vector3(0, -(transform.eulerAngles.z - 90) / 90 * moveSpeed, 0) * Time.deltaTime;

        if (!dutyFinished)
        {
            if (dirToPlayer.x > 1 || PlayerController.dead)
            {
                dutyFinished = true;
                rb.angularVelocity = rotationSpeed;
            }
            else
            {
                Vector3 rotationAdding = dirToPlayer * Time.deltaTime * rotSpeed;
                transform.up += rotationAdding;
            }
        }
        else
        {
            if (transform.eulerAngles.z > 90) rb.AddTorque(-moveSpeed * 15 * Mathf.Abs(forceToAdd.y / Time.unscaledDeltaTime) * Time.deltaTime);
            else rb.AddTorque(moveSpeed * 15 * Mathf.Abs(forceToAdd.y));
        }

        if (transform.eulerAngles.z > 240) forceToAdd = new Vector3(0, 1 * moveSpeed * Time.deltaTime, 0);
        transform.position += forceToAdd;

        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
        rotationSpeed = (transform.eulerAngles.z - lastRotation) / Time.unscaledDeltaTime;
        if (dutyFinished) rotationSpeed = rb.angularVelocity;
    }*/

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
        transform.Find("Boost").gameObject.SetActive(false);
    }
}
