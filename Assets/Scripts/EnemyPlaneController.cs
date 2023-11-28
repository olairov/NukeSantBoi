using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneController : MonoBehaviour
{
    [SerializeField] private Color burnColor;

    [SerializeField] private float moveSpeed, rotSpeed;
    private bool dead, dutyFinished;

    private Transform playerTransform;

    private Rigidbody2D rb;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        rb = transform.GetComponent<Rigidbody2D>();

        transform.position = new Vector3(transform.position.x, Random.Range(Camera.main.ScreenToWorldPoint(Vector3.zero).y, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y), transform.position.z);
    }

    void Update()
    {
        if (!PlayerController.dead && !dutyFinished) { if (!dead) RotateAndMove(); }
        else if (!dead) GoAway();

        if (transform.position.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y - 1 || transform.position.y > Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y + 1) Destroy(gameObject);
    }

    void RotateAndMove()
    {
        Vector3 dirToPlayer = playerTransform.position - transform.position;

        if (dirToPlayer.x > 1) dutyFinished = true;
        else if ((dirToPlayer.y < 0 && transform.eulerAngles.z < 145) || (dirToPlayer.y > 0 && transform.eulerAngles.z > 45))
        {
            Vector3 rotationAdding = dirToPlayer * Time.deltaTime * rotSpeed;
            transform.up += rotationAdding;
        }

        float forceToAddFormula = Mathf.Cos((playerTransform.eulerAngles.z) / 57.3f) * 1f + 0.6f;

        Vector3 forceToAdd = new Vector3(forceToAddFormula * ObjectPassingBy.speedMultiplier * 16, -(transform.eulerAngles.z - 90) / 90 * moveSpeed, 0) * Time.deltaTime;
        transform.position += forceToAdd;

        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
    }

    void GoAway()
    {
        transform.up += Vector3.up * Time.deltaTime;

        Vector3 forceToAdd = new Vector3(0, -(transform.eulerAngles.z - 90) / 90 * moveSpeed * Time.deltaTime, 0);
        transform.position += forceToAdd;
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

        transform.GetComponent<SpriteRenderer>().color = burnColor;
        transform.GetComponent<Collider2D>().enabled = false;
        transform.GetComponent<ObjectPassingBy>().enabled = false;
    }
}
