using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyscraperBuilding : Building, ResetPoolObject
{
    ObjectPool skyscraperUpperPartPool, skyscraperPiecesPool;

    bool alreadyBroken;

    protected override void Start()
    {
        base.Start();

        skyscraperUpperPartPool = GameObject.Find("BuildingPartsContainer/skyscraperUpperPart").GetComponent<ObjectPool>();
        skyscraperPiecesPool = GameObject.Find("ParticlesContainer/skyscraperCutInHalfPieces").GetComponent<ObjectPool>();

        Initialize();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Destroy(Transform otherTransform)
    {
        base.Destroy(otherTransform);

        if (alreadyBroken) return;
        otherTransform.GetComponent<ExplosionController>().CantBreakSkyscraperAgain = true;
        Transform upperPartTransform = SkyscraperFallStats(otherTransform.position.y);
        StartCoroutine(UpperPartFlash(upperPartTransform));
        alreadyBroken = true;
    }

    IEnumerator UpperPartFlash(Transform upperPartTransform)
    {
        SpriteRenderer myUpperSpriteSpriteRenderer = transform.Find("UpperSprite").GetComponent<SpriteRenderer>();
        SpriteRenderer upperPartSpriteRenderer = upperPartTransform.GetComponent<SpriteRenderer>();
        SpriteRenderer upperPartLowerSpriteRenderer = upperPartTransform.Find("LowerSprite").GetComponent<SpriteRenderer>();

        myUpperSpriteSpriteRenderer.color = flashColor;
        upperPartSpriteRenderer.color = flashColor;
        upperPartLowerSpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        myUpperSpriteSpriteRenderer.color = afterFlashColor;
        upperPartSpriteRenderer.color = afterFlashColor;
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
        alreadyBroken = false;
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
