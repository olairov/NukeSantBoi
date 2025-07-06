using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, ResetPoolObject
{
    [SerializeField] private Color flashColor;
    [SerializeField] private Sprite buildingSprite2, buildingSprite3, backBuildingSprite2, backBuildingSprite3;
    static Sprite lastDefaultSprite, lastWideSprite;
    ObjectPool skyscraperUpperPartPool, smokeParticlesPool, shardsParticlesPool, skyscraperPiecesPool, defaultBuildingPool;

    private Material instanceMaterial, backSpriteInstanceMaterial;
    private SpriteRenderer myRenderer;

    private Transform myBackSprite;

    [SerializeField] private float colorValuesIncreaseWhenDie, pushAwayForce;
    private float cameraWidthInUnits, rotationSpeed, fallingSpeed, timeSinceDestruction, pushAwayTime, pushAwayProgress, actualFallRotation, myYdefaultPos;

    public bool dead;
    private bool iStrapSky, imWide, imUpsideDown;

    public bool SetImUpsideDown
    {
        set { imUpsideDown = value; }
    }

    void Start()
    {
        skyscraperUpperPartPool = GameObject.Find("BuildingPartsContainer/skyscraperUpperPart").GetComponent<ObjectPool>();
        smokeParticlesPool = GameObject.Find("ParticlesContainer/buildingSmoke").GetComponent<ObjectPool>();
        shardsParticlesPool = GameObject.Find("ParticlesContainer/buildingShards").GetComponent<ObjectPool>();
        skyscraperPiecesPool = GameObject.Find("ParticlesContainer/skyscraperCutInHalfPieces").GetComponent<ObjectPool>();
        defaultBuildingPool = GameObject.Find("BuildingGenerator/DefaultBuildingGenerator").GetComponent<ObjectPool>();

        myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        myBackSprite = transform.GetChild(1);

        cameraWidthInUnits = (Camera.main.ScreenToWorldPoint(Vector3.one * Screen.width).x - Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        
        Initialize();

        if (transform.name.Contains("Sky")) iStrapSky = true;
        if (transform.name.Contains("Wide"))
        {
            imWide = true;

            // Set the material for the gap shader accordingly to being wide.
            instanceMaterial.SetFloat("_tiling", 4);
            backSpriteInstanceMaterial.SetFloat("_tiling", 4);
        }

        ChooseStats();
    }

    void Update()
    {
        DisplaceBackSprite();
        if (dead) Fall();
        if (pushAwayTime > 0 && !iStrapSky) KeepPushingAway();
    }

    void Initialize()
    {
        // Set the material for the gap shader.
        instanceMaterial = new Material(myRenderer.sharedMaterial);
        myRenderer.material = instanceMaterial;
        instanceMaterial.SetVector("_breakPos", new Vector2(0, 3));

        // Set the material for the gap shader to the back sprite.
        backSpriteInstanceMaterial = new Material(myBackSprite.GetComponent<SpriteRenderer>().sharedMaterial);
        myBackSprite.GetComponent<SpriteRenderer>().material = backSpriteInstanceMaterial;
        backSpriteInstanceMaterial.SetVector("_breakPos", new Vector2(0, 3));
    }

    void ChooseStats()
    {
        float randomY = Random.Range(0f, 2.3f);
        if (imWide) randomY = Random.Range(0f, 1.5f);
        if (iStrapSky) randomY = 0;

        if (imUpsideDown)
        {
            randomY = 14;
            transform.localScale = new Vector3(transform.localScale.x, -Mathf.Abs(transform.localScale.y), transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
        }

        if (imWide && Random.value > 0.999f)
        {
            defaultBuildingPool.GetObject(true).GetComponent<Building>().SetImUpsideDown = true;
        }

        myYdefaultPos = Camera.main.ScreenToWorldPoint(Vector3.zero).y + randomY;
        transform.position = new Vector3(imUpsideDown ? transform.position.x + 4 : transform.position.x, myYdefaultPos, iStrapSky ? -3f : -0.5f);

        if (iStrapSky) return;

        SpriteRenderer mySpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SpriteRenderer myBackSpriteRenderer = myBackSprite.GetComponent<SpriteRenderer>();

        int idx = 0;
        while (true)
        {
            float randomValue = Random.value;

            if (randomValue > 0.66f)
            {
                mySpriteRenderer.sprite = buildingSprite3;
                myBackSpriteRenderer.sprite = backBuildingSprite3;

                if (!imWide)
                {
                    mySpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                    myBackSpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                }
            }
            else if (randomValue > 0.33f)
            {
                mySpriteRenderer.sprite = buildingSprite2;
                myBackSpriteRenderer.sprite = backBuildingSprite2;

                if (!imWide)
                {
                    mySpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                    myBackSpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                }
            }
            else
            {
                if (!imWide)
                {
                    mySpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 14);
                    myBackSpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 14);
                }
            }

            if (imWide && mySpriteRenderer.sprite != lastWideSprite) break;
            else if (!imWide && mySpriteRenderer.sprite != lastDefaultSprite) break;

            idx++;
            if (idx > 99)  // A 100 times loop to avoid excessive repetition if for some reason the sprite always matches the last sprite.
            {
                if (!imWide && (mySpriteRenderer.sprite == buildingSprite2 || mySpriteRenderer.sprite == buildingSprite3))
                {
                    mySpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                    myBackSpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                }

                break;
            }
        }

        if (imWide)
        {
            lastWideSprite = mySpriteRenderer.sprite;

            if (mySpriteRenderer.sprite == buildingSprite2) // In this case, the upper part of the sprite isn't something with what you would collide, so the collider is moved
            {
                BoxCollider2D myColider = GetComponent<BoxCollider2D>();

                myColider.offset = new Vector2(0, -0.5f);
                myColider.size = new Vector2(6, 7);
            }
        }
        else lastDefaultSprite = mySpriteRenderer.sprite;
    }

    public void Destruct(Transform otherTransform)
    {
        Vector2 otherPos = otherTransform.position;

        EnableParticles(otherPos);

        if (dead) return;
        dead = true;

        Transform upperPartTransform = null;
        if (iStrapSky)
        {
            otherTransform.GetComponent<ExplosionController>().CantBreakSkystraperAgain = true;
            upperPartTransform = SkystraperFallStats(otherPos.y);
        }

        StartCoroutine(Flash(upperPartTransform));
        if (!iStrapSky) StartCoroutine(SetGap(otherPos));

        FallStats();
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

    void FallStats()
    {
        rotationSpeed = Random.Range(8f, 12f);
        if (iStrapSky) rotationSpeed *= 0.7f;

        fallingSpeed = -Random.Range(3, 6);
        if (iStrapSky? true : Random.value > 0.5f) rotationSpeed *= -1;
    }

    Transform SkystraperFallStats(float otherYPos)
    {
        float cameraYposOffsetFix = transform.position.y - myYdefaultPos;
        transform.position = new Vector3(transform.position.x, otherYPos - 26f - cameraYposOffsetFix, transform.position.z);

        Transform upperPartTransform = skyscraperUpperPartPool.GetObject(true).transform;
        upperPartTransform.position = new Vector3(transform.position.x, otherYPos + 16f - cameraYposOffsetFix, transform.position.z);
        upperPartTransform.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 7, ForceMode2D.Impulse);
        upperPartTransform.GetComponent<Rigidbody2D>().AddTorque(Random.Range(200, 300));
        if (Random.value > 0.5f) upperPartTransform.GetComponent<Rigidbody2D>().angularVelocity *= -1;

        Transform skyscraperPiecesTransform = skyscraperPiecesPool.GetObject(true).transform;
        skyscraperPiecesTransform.position = new Vector3(transform.position.x, otherYPos - cameraYposOffsetFix, -7);
        /*
        Make both upper and lower part's skystraper script know it has already been broken.
        GetComponent<SkystraperBreakAgain>().OriginalExplosionTransform = otherTransform;
        upperPartTransform.GetComponent<SkystraperBreakAgain>().OriginalExplosionTransform = otherTransform;
        */
        return upperPartTransform;
    }

    void Fall()
    {
        timeSinceDestruction += Time.deltaTime; // Multiplying by this, its fall speed increases exponentially.

        transform.position += new Vector3(0, fallingSpeed * timeSinceDestruction * Time.deltaTime, 0);
        
        actualFallRotation += rotationSpeed * timeSinceDestruction * Time.deltaTime;
        if (iStrapSky) transform.eulerAngles = new Vector3(0, 0, actualFallRotation);
    }

    IEnumerator SetGap(Vector2 otherPos)
    {
        yield return new WaitForSeconds(0.1f);

        Vector2 posToPutDecal = ((Vector2)transform.position - otherPos) / Mathf.PI - new Vector2(0, 1);

        if (!imWide)
        {
            if (posToPutDecal.x < -0.4f) posToPutDecal = new Vector2(-0.4f, posToPutDecal.y);
            else if (posToPutDecal.x > 0.4f) posToPutDecal = new Vector2(0.4f, posToPutDecal.y);

            if (myRenderer.sprite != buildingSprite2 && myRenderer.sprite != buildingSprite3 && posToPutDecal.y < -2) posToPutDecal = new Vector2(posToPutDecal.x, -2f);
            else if (posToPutDecal.y < -2.3f) posToPutDecal = new Vector2(posToPutDecal.x, -2.3f);
        }
        else
        {
            posToPutDecal = new Vector2(Mathf.Clamp(posToPutDecal.x, -1, 1), posToPutDecal.y);

            if (posToPutDecal.y < -2.4f) posToPutDecal = new Vector2(posToPutDecal.x, -2.4f);
        }

        instanceMaterial.SetVector("_breakPos", posToPutDecal);
        backSpriteInstanceMaterial.SetVector("_breakPos", posToPutDecal);
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
            transform.Find("UpperSprite").GetComponent<SpriteRenderer>().color = flashColor;
            upperPartTransform.Find("LowerSprite").GetComponent<SpriteRenderer>().color = flashColor;
        }

        mySpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(0.1f);

        Color lastColor = new Color(originalCol.r + colorValuesIncreaseWhenDie, originalCol.g + colorValuesIncreaseWhenDie, originalCol.b + colorValuesIncreaseWhenDie);

        mySpriteRenderer.color = lastColor;
        if (upperPartTransform != null)
        {
            Color lastUpperPartColor = new Color(originalUpperPartColor.r + colorValuesIncreaseWhenDie, originalUpperPartColor.g + colorValuesIncreaseWhenDie, originalUpperPartColor.b + colorValuesIncreaseWhenDie);

            upperPartSpriteRenderer.color = lastUpperPartColor;
            upperPartTransform.Find("LowerSprite").GetComponent<SpriteRenderer>().color = lastUpperPartColor;
            transform.Find("UpperSprite").GetComponent<SpriteRenderer>().color = lastColor;
        }
    }

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

        transform.eulerAngles = new Vector3(0, 0, dead? pushAwayProgress * pushAwayLerpProgress * 3 + actualFallRotation : pushAwayProgress * pushAwayLerpProgress);
    }


    // Reset Pooled Object State

    public void ResetState()
    {
        rotationSpeed = fallingSpeed = timeSinceDestruction = pushAwayTime = pushAwayProgress = actualFallRotation = myYdefaultPos = 0;
        imUpsideDown = dead = false;
        transform.eulerAngles = Vector3.zero;

        Initialize();
        ChooseStats();
    }
}
