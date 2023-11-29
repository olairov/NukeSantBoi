using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody2D rb;

    private Vector3 lastFramePos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        lastFramePos = transform.position;
    }

    void Update()
    {
        if (transform.position.y < Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y - 0.5f) Explode();

        UpdateRotation();
    }

    void UpdateRotation()
    {
        Vector2 velocity = (transform.position - lastFramePos) / Time.deltaTime;

        /*transform.up = rb.velocity + new Vector2(5 * ObjectPassingBy.speedMultiplier, 0);
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 110);*/

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
        if (other.gameObject.layer == 3)
        {
            Explode();
        }
    }
}
