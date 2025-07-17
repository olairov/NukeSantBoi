using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotAirBalloonScript : Entity, ResetPoolObject
{
    [SerializeField] private Color burnColor, possibleColor1, possibleColor2, possibleColor3, possibleColor4, possibleColor5;

    [SerializeField] private GameObject obstaclePrefab;

    private Rigidbody2D rb;

    Vector2 actualDirection;
    Color originalBasketColor;
    SpriteRenderer basketSpriteRenderer;

    [SerializeField] private float speed, rotSpeed, pushForce, speedMax, slowDownSpeed;
    private float randRotDelay;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        basketSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        originalBasketColor = basketSpriteRenderer.color;

        Initialize();
    }

    protected override void Update()
    {
        base.Update();

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
        basketSpriteRenderer.color = burnColor;
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


    // Reset Pooled Object State

    public override void ResetState()
    {
        base.ResetState();

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        actualDirection = Vector2.zero;
        rb.rotation = 0f;
        rb.gravityScale = 0;
        basketSpriteRenderer.color = originalBasketColor;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<ObjectPassingBy>().moveInFixedUpdate = false;
    }

    public override void Initialize()
    {
        base.Initialize();

        float randValue = Random.value;

        if (randValue > 0.8f) transform.GetComponent<SpriteRenderer>().color = possibleColor1;
        else if (randValue > 0.6f) transform.GetComponent<SpriteRenderer>().color = possibleColor2;
        else if (randValue > 0.4f) transform.GetComponent<SpriteRenderer>().color = possibleColor3;
        else if (randValue > 0.2f) transform.GetComponent<SpriteRenderer>().color = possibleColor4;
        else transform.GetComponent<SpriteRenderer>().color = possibleColor5;

        randRotDelay = Random.Range(0f, 3f);
    }
}
