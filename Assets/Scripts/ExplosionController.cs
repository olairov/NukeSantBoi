using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private SpriteRenderer mySprite;

    private float timeStart, Acolor = 1;

    private bool shortExplosion;

    void Start()
    {
        mySprite = transform.GetComponent<SpriteRenderer>();
        timeStart = Time.time;

        transform.position = new Vector3(transform.position.x, transform.position.y, -2);
        transform.parent = GameObject.Find("ExplosionContainer").transform;
    }

    private void Update()
    {
        if (Time.time - timeStart > 0.05f)
        {
            transform.GetComponent<Collider2D>().enabled = true;

            Acolor -= Time.deltaTime * 1.5f;
            mySprite.color = new Color(mySprite.color.r, mySprite.color.g, mySprite.color.b, Acolor);
        }
        if (Time.time - timeStart > 0.45f) transform.GetComponent<Collider2D>().enabled = false;
        if (shortExplosion && Time.time - timeStart > 0.2f) transform.GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Skystraper") shortExplosion = true;
    }
}
