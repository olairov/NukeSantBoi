using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    [SerializeField] private Color burnColor;

    private Rigidbody2D rb;

    private Vector3 actualDirection;

    [SerializeField] private float speed, rotSpeed;
    private float timeForChange, timeForRotChange = 1;
    private int rotationDir = 1;

    private bool dead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        transform.position = new Vector3(transform.position.x, Random.Range(Camera.main.ScreenToWorldPoint(Vector3.zero).y + 4, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - 1), transform.position.z);
    }

    void Update()
    {
        rb.AddForce(actualDirection * Time.deltaTime * speed);
        rb.AddTorque(Time.deltaTime * rotationDir * rotSpeed);

        RotionChange();
        DirChange();
    }

    void RotionChange()
    {
        timeForRotChange -= Time.deltaTime;
        if (timeForRotChange > 0) return;

        rotationDir *= -1;

        timeForRotChange = 2;
    }

    void DirChange()
    {
        timeForChange -= Time.deltaTime;
        if (timeForChange > 0) return;

        actualDirection = new Vector3(Random.Range(1f, 1f), Random.Range(1f, 1f), 0).normalized;

        timeForChange = Random.Range(0.5f, 3f);
    }

    void Die()
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Explosion") Die();
    }
}
