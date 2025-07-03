using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotAirBalloonScript : MonoBehaviour
{
    [SerializeField] private Color burnColor, possibleColor1, possibleColor2, possibleColor3, possibleColor4, possibleColor5;

    [SerializeField] private GameObject obstaclePrefab;
    private Transform hotAirBalloonsContainer;

    private Rigidbody2D rb;

    private Vector2 actualDirection;

    [SerializeField] private float speed, rotSpeed, pushForce, speedMax, slowDownSpeed;
    private float randRotDelay;

    private bool dead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hotAirBalloonsContainer = GameObject.Find("ObstacleGenerator/HotAirBalloonGenerator").transform;

        ChoseStats();
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

        randRotDelay = Random.Range(0f, 3f);
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
        rb.AddForce(new Vector2(Random.Range(3, 7), 6), ForceMode2D.Impulse);

        GetComponent<SpriteRenderer>().color = burnColor;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = burnColor;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<ObjectPassingBy>().moveInFixedUpdate = true;
    }

    public void PushAway(Vector2 direction, float distance)
    {
        rb.AddForce(direction * pushForce / distance, ForceMode2D.Impulse);
    }

    public void SlowDown()
    {
        if (!dead) rb.velocity *= 1 - Time.deltaTime * slowDownSpeed;
    }
}
