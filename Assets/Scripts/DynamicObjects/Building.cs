using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private Color flashColor;
    [SerializeField] private GameObject skystraperUpperPart, smokeParticles, shardsParticles;

    private Transform particlesContainer;

    private float rotationSpeed, fallingSpeed, timeSinceDestruction;

    public bool dead;
    private bool iStrapSky;

    void Start()
    {
        particlesContainer = GameObject.Find("ParticlesContainer").transform;

        if (transform.name.Contains("Sky")) iStrapSky = true;
        CreateStats();
    }

    void Update()
    {
        if (dead) Fall();
    }

    void CreateStats()
    {
        float randomY = Random.Range(-3f, 3f);
        if (transform.name.StartsWith("Wide")) randomY = Random.Range(-1f, 2f);
        if (iStrapSky) randomY = 0;

        transform.position = new Vector3(transform.position.x, Camera.main.ScreenToWorldPoint(Vector3.zero).y + randomY, transform.position.z);
    }

    public void Destruct(Vector2 otherPos)
    {
        EnableParticles(otherPos);

        if (dead) return;
        dead = true;

        Transform upperPartTransform = null;
        if (iStrapSky) upperPartTransform = SkystraperFallStats(otherPos.y);

        StartCoroutine(Flash(upperPartTransform));

        FallStats();
    }

    void EnableParticles(Vector2 explosionPos)
    {
        Instantiate(smokeParticles, new Vector3(transform.position.x, Camera.main.ScreenToWorldPoint(Vector3.zero).y -1, -20), Quaternion.identity, particlesContainer);

        float shardsZrotation = -(explosionPos.x - transform.position.x) * 30 + 90;
        if (shardsZrotation < 20) shardsZrotation = 20;
        else if (shardsZrotation > 160) shardsZrotation = 160;
        Instantiate(shardsParticles, new Vector3(explosionPos.x, explosionPos.y - 0.5f, shardsParticles.transform.position.z), Quaternion.Euler(0, 0, shardsZrotation), particlesContainer);
    }

    void FallStats()
    {
        rotationSpeed = Random.Range(8f, 16f);
        fallingSpeed = -Random.Range(3, 6);
        if (Random.value > 0.5f) rotationSpeed *= -1;
    }

    Transform SkystraperFallStats(float otherY)
    {
        transform.position = new Vector3(transform.position.x, otherY - 25, transform.position.z);

        Transform upperPartTransform = Instantiate(skystraperUpperPart, transform.parent).transform;
        upperPartTransform.position = new Vector3(transform.position.x, otherY + 15, transform.position.z);
        upperPartTransform.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 7, ForceMode2D.Impulse);
        upperPartTransform.GetComponent<Rigidbody2D>().AddTorque(Random.Range(80, 180));
        if (Random.value > 0.5f) upperPartTransform.GetComponent<Rigidbody2D>().angularVelocity *= -1;

        return upperPartTransform;
    }

    void Fall()
    {
        timeSinceDestruction += Time.deltaTime;

        transform.position += new Vector3(0, fallingSpeed * timeSinceDestruction * Time.deltaTime, 0);
        transform.eulerAngles += new Vector3(0, 0, rotationSpeed * timeSinceDestruction * Time.deltaTime);
    }

    IEnumerator Flash(Transform upperPartTransform)
    {
        SpriteRenderer mySpriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        Color originalCol = mySpriteRenderer.color;
        SpriteRenderer upperPartSpriteRenderer = null;
        Color originalUpperPartColor = Color.white;

        if (upperPartTransform != null)
        {
            upperPartSpriteRenderer = upperPartTransform.GetComponent<SpriteRenderer>();
            originalUpperPartColor = upperPartSpriteRenderer.color;
            upperPartSpriteRenderer.color = flashColor;
        }

        mySpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(0.1f);

        mySpriteRenderer.color = originalCol;
        if (upperPartTransform != null) upperPartSpriteRenderer.color = originalUpperPartColor;
    }
}
