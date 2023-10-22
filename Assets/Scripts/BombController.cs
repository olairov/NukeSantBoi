using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    void Update()
    {
        if (transform.position.y < Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y - 0.5f) Explode();
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
