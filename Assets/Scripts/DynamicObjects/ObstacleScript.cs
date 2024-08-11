using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    [SerializeField] private Color burnColor, possibleColor1, possibleColor2, possibleColor3, possibleColor4, possibleColor5;

    [SerializeField] private GameObject obstaclePrefab;
    private Transform obstaclesContainer;

    private Rigidbody2D rb;

    private Vector2 actualDirection;

    [SerializeField] private float speed, rotSpeed, pushForce, speedMax;
    private float randRotDelay;

    private bool dead, imDuplicated;
    public bool SetImDuplicated
    {
        set { imDuplicated = value; }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        obstaclesContainer = GameObject.Find("ObstaclesContainer").transform;

        ChoseStats();

        randRotDelay = Random.Range(0f, 3f);
        transform.position = new Vector3(transform.position.x, Random.Range(Camera.main.ScreenToWorldPoint(Vector3.zero).y + 4, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - 1), transform.position.z);
    }

    void Update()
    {
        if (rb.velocity.magnitude < speedMax) rb.AddForce(actualDirection * Time.deltaTime * speed);

        if (!dead) transform.eulerAngles = new Vector3(0, 0, Mathf.Cos(Time.time * rotSpeed + randRotDelay) * 12);

        DirChange(Vector2.zero);
    }

    public void DirChange(Vector2 direction) // Give direction a Vector2.zero for a random direction election.
    {
        if (direction == Vector2.zero)
        {
            actualDirection += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            //actualDirection = Vector3.ClampMagnitude(actualDirection, 1);
        }
        else
        {
            actualDirection += direction;
        }

        Vector2.ClampMagnitude(actualDirection, 1); // Prevent it from reaching too high speeds;
    }

    void ChoseStats()
    {
        float randValue = Random.value;

        if (randValue > 0.8f) transform.GetComponent<SpriteRenderer>().color = possibleColor1;
        else if (randValue > 0.6f) transform.GetComponent<SpriteRenderer>().color = possibleColor2;
        else if (randValue > 0.4f) transform.GetComponent<SpriteRenderer>().color = possibleColor3;
        else if (randValue > 0.2f) transform.GetComponent<SpriteRenderer>().color = possibleColor4;
        else transform.GetComponent<SpriteRenderer>().color = possibleColor5;

        if (Random.value > 0.9f && !imDuplicated)
        {
            Vector3 otherPos = new Vector3(transform.position.x, transform.position.y < 0 ? transform.position.y + 5 : transform.position.y - 5, transform.position.z);
            Instantiate(obstaclePrefab, otherPos, Quaternion.identity, obstaclesContainer).GetComponent<ObstacleScript>().SetImDuplicated = true;
        }
    }

    public void Die()
    {
        if (dead) return;
        dead = true;

        rb.angularDrag = 0;
        rb.gravityScale = 2;
        rb.drag = 0;
        rb.velocity = Vector2.zero;

        rb.AddTorque(Random.Range(80f, 240f));
        if (Random.value > 0.5f) rb.angularVelocity *= -1;
        rb.AddForce(new Vector2(Random.Range(-2, 3), 5), ForceMode2D.Impulse);

        transform.GetComponent<SpriteRenderer>().color = burnColor;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = burnColor;
        transform.GetComponent<Collider2D>().enabled = false;
        transform.GetComponent<ObjectPassingBy>().enabled = false;
    }

    public void PushAway(Vector2 direction, float distance)
    {
        rb.AddForce(direction * pushForce / distance, ForceMode2D.Impulse);
    }
}
