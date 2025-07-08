using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour, ResetPoolObject
{
    [SerializeField] ObjectPool explosionPool;

    [SerializeField] private float bombThrowSpeed;

    private Rigidbody2D rb;

    private bool alreadyExploded; // I'm not sure this variable is needed, but sometimes, two explosions are generated. It's just in case Destroy() is not instant.

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        explosionPool = GameObject.Find("ExplosionsContainer").GetComponent<ObjectPool>();

        Initialize();
    }

    void Update()
    {
        if (transform.position.y < Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y - 0.5f) Explode();

        UpdateRotation();
    }

    public void SetDirection(Vector2 normalDir, Vector2 yAdder, float playerRotiationIndicator)
    {
        float speedMultiplier = ObjectPassingBy.realSpeedMultiplier / 2f + 0.5f; // Why did I do this
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

        explosionPool.GetObject(true).transform.position = transform.position;

        if (GetComponent<PooledObject>() != null) GetComponent<PooledObject>().ReturnToPool(gameObject);
        else
        {
            Debug.LogWarning("Pooled Object script not found in " + transform.name);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 3 || other.gameObject.layer == 6) Explode();
    }


    // Reset Pooled Object State

    public void ResetState()
    {
        alreadyExploded = false;
        rb.velocity = Vector2.zero;
        transform.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetComponent<Collider2D>().enabled = true;
    }

    public void Initialize()
    {
        transform.position += new Vector3(0, 0, 2);
        transform.position += (Vector3)rb.velocity * 0.06f;
    }
}