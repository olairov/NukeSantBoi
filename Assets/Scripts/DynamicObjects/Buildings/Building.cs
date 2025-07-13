using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity
{
    [SerializeField] float pushAwayForce, fallSpeedMin, fallSpeedMax, fallRotationSpeedMax, fallRotationSpeedmMin;
    [SerializeField] bool cantFallBackwards;

    public Color flashColor, afterFlashColor;
    ObjectPool smokeParticlesPool, shardsParticlesPool;
    public Transform myBackSprite;

    public float flashDuration;
    float cameraWidthInUnits, pushAwayTime, rotationSpeed, pushAwayProgress, fallingSpeed, timeSinceDestruction, actualFallRotation;
    public bool imUpsideDown, iStrapSky;

    protected override void Start()
    {
        base.Start();
        
        cameraWidthInUnits = Camera.main.ScreenToWorldPoint(Vector3.one * Screen.width).x - Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        myBackSprite = transform.GetChild(1);

        if (transform.name.Contains("Sky")) iStrapSky = true;

        smokeParticlesPool = GameObject.Find("ParticlesContainer/buildingSmoke").GetComponent<ObjectPool>();
        shardsParticlesPool = GameObject.Find("ParticlesContainer/buildingShards").GetComponent<ObjectPool>();
    }

    protected override void Update()
    {
        base.Update();

        DisplaceBackSprite();
        if (pushAwayTime > 0) KeepPushingAway();
        if (dead) Fall();
    }

    public int ChooseBuildingSprite(int numberOfPossibleSprites, int lastSprite) // THIS ALWAYS GETS IN AN INFINITE LOOP
    {
        // We create as many float ranges / intervals between 0 & 1 as the number of different sprites there are.
        // Each range represents the weight of each sprite when choosing one (all sprites' weights are equal).
        float rangeOfChance = 1f / numberOfPossibleSprites;

        int timesTried = 0;
        while (true) // This will loop until a random sprite different from the last sprite is chosen.
        {
            float randomValue = Random.value;

            for (int idx = 0; idx < numberOfPossibleSprites; idx++)
            {
                // For each sprite, we test if randomValue is inside this sprite's interval.
                // The beginning of this interval is the end of the last interval,
                // and the end of this interval is the end of the last interval added to rangeOfChance
                float chanceThresholdForThisSprite = rangeOfChance * (idx + 1);
                if (randomValue > chanceThresholdForThisSprite) continue;
                
                // It's a 100 times loop to avoid excessive looping if for some reason the chosen sprite always matches the last sprite.
                if (idx == lastSprite && timesTried < 100) break;
                return idx;
            }
        }
    }

    public virtual void Destroy(Transform otherTransform)
    {
        EnableParticles(otherTransform.position);

        if (dead) return;
        dead = true;

        StartCoroutine(Flash());

        FallStats();
    }

    IEnumerator Flash()
    {
        SpriteRenderer mySpriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        mySpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        mySpriteRenderer.color = afterFlashColor;
    }

    void EnableParticles(Vector2 explosionPos)
    {
        smokeParticlesPool.GetObject(true).transform.position = new Vector3(transform.position.x, Camera.main.ScreenToWorldPoint(Vector3.zero).y - 1, -20);

        float shardsZrotation = -(explosionPos.x - transform.position.x) * 30 + 90;
        if (shardsZrotation < 20) shardsZrotation = 20;
        else if (shardsZrotation > 160) shardsZrotation = 160;

        Transform shardsTransform = shardsParticlesPool.GetObject(true).transform;
        shardsTransform.position = new Vector3(explosionPos.x, explosionPos.y - 0.5f, transform.position.z - 0.5f);
        shardsTransform.eulerAngles = new Vector3(0, 0, shardsZrotation);
    }

    void DisplaceBackSprite()
    {
        myBackSprite.localPosition = new Vector3(Mathf.Lerp(0.5f, -0.5f, transform.position.x / cameraWidthInUnits + 0.5f), myBackSprite.localPosition.y, myBackSprite.localPosition.z);
    }

    public void FallStats()
    {
        rotationSpeed = -Random.Range(fallRotationSpeedmMin, fallRotationSpeedMax);

        fallingSpeed = -Random.Range(fallSpeedMin, fallSpeedMax);
        if (!cantFallBackwards && Random.value > 0.5f) rotationSpeed *= -1;
    }

    public void Fall()
    {
        timeSinceDestruction += Time.deltaTime; // Multiplying by this, its fall speed increases exponentially.

        transform.position += new Vector3(0, fallingSpeed * timeSinceDestruction * Time.deltaTime, 0);

        actualFallRotation += rotationSpeed * timeSinceDestruction * Time.deltaTime;
        if (iStrapSky) transform.eulerAngles = new Vector3(0, 0, actualFallRotation);
    }


    // Being Pushed By An Explosion:

    public void PushAway(float Xdirection, float distance)
    {
        pushAwayProgress = -Xdirection * pushAwayForce / (distance * 2.5f);
        if (imUpsideDown) pushAwayForce *= -1;

        pushAwayTime = 1;
    }

    private void KeepPushingAway()
    {
        // This makes pushAwayTime decrease less every frame. At first it decreases fast, and then very slow, to create a smooth effect.
        pushAwayTime /= 1 + (Time.deltaTime * 3);

        float pushAwayLerpProgress = (-pushAwayTime + 1) * 5;
        if (pushAwayTime < 0.8f) pushAwayLerpProgress = pushAwayTime * 1.111f; // I should have commented this before.

        transform.eulerAngles = new Vector3(0, 0, dead ? pushAwayProgress * pushAwayLerpProgress * 3 + actualFallRotation : pushAwayProgress * pushAwayLerpProgress);
    }


    // Reset Pooled Object State

    public override void ResetState()
    {
        base.ResetState();

        pushAwayTime = pushAwayProgress = fallingSpeed = timeSinceDestruction = actualFallRotation = rotationSpeed = 0;
        imUpsideDown = false;

        transform.eulerAngles = Vector3.zero;
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
