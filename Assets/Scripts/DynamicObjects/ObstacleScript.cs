using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    [SerializeField] private Color burnColor, possibleColor1, possibleColor2, possibleColor3, possibleColor4, possibleColor5;

    [SerializeField] private GameObject obstaclePrefab;
    private Transform obstaclesContainer;

    private Rigidbody2D rb;

    private Vector3 actualDirection;

    [SerializeField] private float speed, rotSpeed, pushForce;
    private float randRotDelay;

    private bool dead;

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
        rb.AddForce(actualDirection * Time.deltaTime * speed);

        if (!dead) transform.eulerAngles = new Vector3(0, 0, Mathf.Cos(Time.time * rotSpeed + randRotDelay) * 12);

        DirChange();
    }

    void DirChange()
    {
        actualDirection += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * Time.deltaTime * 50;
        actualDirection = Vector3.ClampMagnitude(actualDirection, 1);
    }

    void ChoseStats()
    {
        float randValue = Random.value;

        if (randValue > 0.8f) transform.GetComponent<SpriteRenderer>().color = possibleColor1;
        else if (randValue > 0.6f) transform.GetComponent<SpriteRenderer>().color = possibleColor2;
        else if (randValue > 0.4f) transform.GetComponent<SpriteRenderer>().color = possibleColor3;
        else if (randValue > 0.2f) transform.GetComponent<SpriteRenderer>().color = possibleColor4;
        else transform.GetComponent<SpriteRenderer>().color = possibleColor5;

        if (Random.value > 0.9f)
        {
            Instantiate(obstaclePrefab, new Vector3(transform.position.x + 8f, transform.position.y < 0 ? transform.position.y + 5 : transform.position.y - 5, transform.position.z), Quaternion.identity, obstaclesContainer);
        }
    }

    public void Die()
    {
        if (dead) return;
        dead = true;

        rb.angularDrag = 0;
        rb.gravityScale = 2;
        rb.drag = 0;

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
