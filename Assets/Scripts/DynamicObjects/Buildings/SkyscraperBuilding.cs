using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyscraperBuilding : Building, ResetPoolObject
{
    ObjectPool skyscraperUpperPartPool, skyscraperPiecesPool;

    protected override void Start()
    {
        base.Start();

        skyscraperUpperPartPool = GameObject.Find("BuildingPartsContainer/skyscraperUpperPart").GetComponent<ObjectPool>();
        skyscraperPiecesPool = GameObject.Find("ParticlesContainer/skyscraperCutInHalfPieces").GetComponent<ObjectPool>();

        if (transform.name.Contains("Sky")) iStrapSky = true;

        Initialize();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Destroy(Transform otherTransform)
    {
        base.Destroy(otherTransform);

        otherTransform.GetComponent<ExplosionController>().CantBreakSkyscraperAgain = true;
        Transform upperPartTransform = SkyscraperFallStats(otherTransform.position.y);
        StartCoroutine(UpperPartFlash(upperPartTransform));
    }

    IEnumerator UpperPartFlash(Transform upperPartTransform)
    {
        SpriteRenderer myUpperSpriteSpriteRenderer = transform.Find("UpperSprite").GetComponent<SpriteRenderer>();
        SpriteRenderer upperPartSpriteRenderer = upperPartTransform.GetComponent<SpriteRenderer>();
        SpriteRenderer upperPartUpperSpriteRenderer = upperPartTransform.Find("UpperSprite").GetComponent<SpriteRenderer>();
        SpriteRenderer upperPartLowerSpriteRenderer = upperPartTransform.Find("LowerSprite").GetComponent<SpriteRenderer>();
        Color upperPartUpperSpriteColor = upperPartUpperSpriteRenderer.color; // The upper part's upper sprite has a unique color.

        myUpperSpriteSpriteRenderer.color = flashColor;
        upperPartSpriteRenderer.color = flashColor;
        upperPartUpperSpriteRenderer.color = flashColor;
        upperPartLowerSpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        myUpperSpriteSpriteRenderer.color = afterFlashColor;
        upperPartSpriteRenderer.color = afterFlashColor;
        upperPartUpperSpriteRenderer.color = upperPartUpperSpriteColor;
        upperPartLowerSpriteRenderer.color = afterFlashColor;
    }

    Transform SkyscraperFallStats(float otherYPos)
    {
        float cameraYposOffsetFix = transform.position.y;
        transform.position = new Vector3(transform.position.x, otherYPos - 26f - cameraYposOffsetFix, transform.position.z);

        Transform upperPartTransform = skyscraperUpperPartPool.GetObject(true).transform;
        upperPartTransform.position = new Vector3(transform.position.x, otherYPos + 16f - cameraYposOffsetFix, transform.position.z);
        upperPartTransform.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 7, ForceMode2D.Impulse);
        upperPartTransform.GetComponent<Rigidbody2D>().AddTorque(Random.Range(200, 300));
        if (Random.value > 0.5f) upperPartTransform.GetComponent<Rigidbody2D>().angularVelocity *= -1;

        Transform skyscraperPiecesTransform = skyscraperPiecesPool.GetObject(true).transform;
        skyscraperPiecesTransform.position = new Vector3(transform.position.x, otherYPos - cameraYposOffsetFix, -7);
        
        return upperPartTransform;
    }


    // Reset Pooled Object State

    public override void ResetState()
    {
        base.ResetState();
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
