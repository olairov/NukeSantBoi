using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody2D rb;

    private Vector3 lastFramePos;

    private bool notFirstFrame, inPlayerYet = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        transform.position += new Vector3(0, 0, 2);
        lastFramePos = transform.position;

        transform.position += (Vector3)rb.velocity * 0.06f;
    }

    void Update()
    {
        if (transform.position.y < Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y - 0.5f) Explode();

        UpdateRotation();
    }

    void UpdateRotation()
    {
        transform.up = rb.velocity + new Vector2(5 * ObjectPassingBy.realSpeedMultiplier, 0);
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 110);

        lastFramePos = transform.position;
    }

    void Explode()
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetComponent<Collider2D>().enabled = false;

        Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 3) Explode();
    }
}