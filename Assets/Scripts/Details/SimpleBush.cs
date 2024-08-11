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
        SpriteRenderer mySprite = transform.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(imFartherBush ? transform.position.x + Random.Range(1f, 3f) : transform.position.x,
            Random.Range(-7f, -6f) + (imFartherBush ? 2.5f : 0), imFartherBush ? 12.5f : 10);

        transform.localScale = new Vector3(Random.value > 0.5 ? transform.localScale.x : -transform.localScale.x, transform.localScale.y, 0) * Random.Range(1f, 1.4f);

        if (imCloserBush)
        {
            GetComponent<ObjectPassingBy>().passingSpeed = 6.5f;
            transform.localScale *= 1.5f;
            transform.position = new Vector3(transform.position.x, Random.Range(-8.3f, -9.3f), -12.5f);
            mySprite.color = new Color(Random.Range(0.2f, 0.4f), 0.85f, 0);
        }
        else if (imFartherBush)
        {
            GetComponent<ObjectPassingBy>().passingSpeed = 3.5f;
            mySprite.color = new Color(Random.Range(0.2f, 0.4f), 0.55f, 0);
        }
        else mySprite.color = new Color(Random.Range(0.2f, 0.4f), 0.65f, 0);

        if (Random.value > 0.6f && !imFartherBush && !imCloserBush) Instantiate(gameObject, transform.parent).GetComponent<SimpleBush>().imFartherBush = true;

        if (Random.value > 0.7 && !imFartherBush && !imCloserBush) Instantiate(gameObject, transform.parent).GetComponent<SimpleBush>().imCloserBush = true;
    }
}
