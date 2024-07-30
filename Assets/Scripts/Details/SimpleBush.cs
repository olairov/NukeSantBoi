using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBush : MonoBehaviour
{
    public bool imFartherBush, imCloserBush;

    void Start()
    {
        InitializeStats();
    }

    void InitializeStats()
    {
        transform.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.2f, 0.4f), 0.65f, 0);
        transform.position = new Vector3(imFartherBush ? transform.position.x + Random.Range(1f, 3f) : transform.position.x,
            Random.Range(-6.5f, -5.5f) + (imFartherBush ? 2f : 0), imFartherBush ? 12.5f : 10);
        transform.localScale *= Random.Range(1f, 1.4f);

        if (imCloserBush)
        {
            transform.GetComponent<ObjectPassingBy>().passingSpeed = 6.5f;
            transform.localScale *= 1.5f;
            transform.position = new Vector3(transform.position.x, Random.Range(-8f, -9f), -1);
        }

        if (imFartherBush) transform.GetComponent<ObjectPassingBy>().passingSpeed = 3.5f;

        if (Random.value > 0.6f && !imFartherBush && !imCloserBush) Instantiate(gameObject, transform.parent).GetComponent<SimpleBush>().imFartherBush = true;

        if (Random.value > 0.8 && !imFartherBush && !imCloserBush) Instantiate(gameObject, transform.parent).GetComponent<SimpleBush>().imCloserBush = true;
    }
}
