using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody2D rb;

    [SerializeField] private float bombThrowSpeed;

    private bool alreadyExploded; // I'm not sure this variable is needed, but sometimes, two explosions are generated. It's just in case Destroy() is not instant.

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        transform.position += new Vector3(0, 0, 2);

        transform.position += (Vector3)rb.velocity * 0.06f;
    }

    void Update()
    {
        if (transform.position.y < Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y - 0.5f) Explode();

        UpdateRotation();
    }

    public void SetDirection(Vector2 normalDir, Vector2 yAdder, float playerRotiationIndicator)
    {
        float speedMultiplier = ObjectPassingBy.realSpeedMultiplier / 2f + 0.5f;
        GetComponent<Rigidbody2D>().velocity = normalDir * speedMultiplier * bombThrowSpeed + yAdder;

        GetComponent<ObjectPassingBy>().OriginalMovement = playerRotiationIndicator * 5;
    }

    void UpdateRotation()
    {
        transform.up = rb.velocity + new Vector2(5 * ObjectPassingBy.realSpeedMultiplier, 0);
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 110);
    }

    void Explode()
    {
        if (alreadyExploded) return;

        alreadyExploded = true;

        transform.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetComponent<Collider2D>().enabled = false;

        Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 3 || other.gameObject.layer == 6) Explode();
    }
}