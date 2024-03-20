using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private Color flashColor;
    [SerializeField] private GameObject skystraperUpperPart, smokeParticles, shardsParticles, skyStraperPieces, buildingPrefab;
    [SerializeField] private Sprite buildingSprite2, buildingSprite3, backBuildingSprite2, backBuildingSprite3;
    public Sprite lastSprite;

    private MapGenerator mapGeneratorScript;

    private Transform particlesContainer, myBackSprite;

    [SerializeField] private float colorValuesIncreaseWhenDie, pushAwayForce;
    private float rotationSpeed, fallingSpeed, timeSinceDestruction, cameraWidthInUnits, pushAwayTime, pushAwayProgress;

    public bool dead;
    private bool iStrapSky, isWide, imUpsideDown;

    public bool SetImUpsideDown
    {
        set { imUpsideDown = value; }
    }

    void Start()
    {
        mapGeneratorScript = GameObject.Find("__________________Map___________________").GetComponent<MapGenerator>();
        particlesContainer = GameObject.Find("ParticlesContainer").transform;
        myBackSprite = transform.GetChild(1);

        cameraWidthInUnits = (Camera.main.ScreenToWorldPoint(Vector3.one * Screen.width).x - Camera.main.ScreenToWorldPoint(Vector3.zero).x);

        if (transform.name.Contains("Sky")) iStrapSky = true;
        if (transform.name.Contains("Wide")) isWide = true;
        CreateStats();
    }

    void Update()
    {
        DisplaceBackSprite();
        if (dead) Fall();
        if (pushAwayTime > 0 && !iStrapSky) KeepPushingAway();
    }

    void CreateStats()
    {
        float randomY = Random.Range(0f, 3f);
        if (isWide) randomY = Random.Range(0f, 2f);
        if (iStrapSky) randomY = 0;

        if (imUpsideDown)
        {
            randomY = 14;
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        }

        if (isWide && Random.value > 0.999f) Instantiate(buildingPrefab, transform.parent).GetComponent<Building>().SetImUpsideDown = true;

        transform.position = new Vector3(imUpsideDown ? transform.position.x + 4 : transform.position.x, Camera.main.ScreenToWorldPoint(Vector3.zero).y + randomY, iStrapSky ? -3f : -0.5f);

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

                if (!isWide)
                {
                    mySpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                    myBackSpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                }
            }
            else if (randomValue > 0.33f)
            {
                mySpriteRenderer.sprite = buildingSprite2;
                myBackSpriteRenderer.sprite = backBuildingSprite2;

                if (!isWide)
                {
                    mySpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                    myBackSpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                }
            }
            else
            {
                if (!isWide)
                {
                    mySpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 14);
                    myBackSpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 14);
                }
            }

            if (isWide && mySpriteRenderer.sprite != mapGeneratorScript.lastWideBuildingSprite) break;
            else if (!isWide && mySpriteRenderer.sprite != mapGeneratorScript.lastBuildingSprite) break;

            idx++;
            if (idx > 9)
            {
                if (!isWide && (mySpriteRenderer.sprite == buildingSprite2 || mySpriteRenderer.sprite == buildingSprite3))
                {
                    mySpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                    myBackSpriteRenderer.size = new Vector2(transform.GetComponentInChildren<SpriteRenderer>().size.x, 10);
                }

                break;
            }
        }

        if (isWide) mapGeneratorScript.lastWideBuildingSprite = mySpriteRenderer.sprite;
        else mapGeneratorScript.lastBuildingSprite = mySpriteRenderer.sprite;
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

    void DisplaceBackSprite()
    {
        myBackSprite.localPosition = new Vector3(Mathf.Lerp(0.5f, -0.5f, transform.position.x / cameraWidthInUnits + 0.5f), myBackSprite.localPosition.y, myBackSprite.localPosition.z);
    }

    void FallStats()
    {
        rotationSpeed = Random.Range(8f, 16f);
        fallingSpeed = -Random.Range(3, 6);
        if (iStrapSky? true : Random.value > 0.5f) rotationSpeed *= -1;
    }

    Transform SkystraperFallStats(float otherY)
    {
        transform.position = new Vector3(transform.position.x, otherY - 25.7f, transform.position.z);

        Transform upperPartTransform = Instantiate(skystraperUpperPart, transform.parent).transform;
        upperPartTransform.position = new Vector3(transform.position.x, otherY + 15.7f, transform.position.z);
        upperPartTransform.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 7, ForceMode2D.Impulse);
        upperPartTransform.GetComponent<Rigidbody2D>().AddTorque(Random.Range(80, 180));
        if (Random.value > 0.5f) upperPartTransform.GetComponent<Rigidbody2D>().angularVelocity *= -1;

        Instantiate(skyStraperPieces, new Vector3(transform.position.x, otherY, -7), Quaternion.identity, particlesContainer);

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
            transform.Find("BrokenSprite").GetComponent<SpriteRenderer>().color = flashColor;
            upperPartTransform.Find("BrokenSprite").GetComponent<SpriteRenderer>().color = flashColor;
        }

        mySpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(0.1f);

        Color lastColor = new Color(originalCol.r + colorValuesIncreaseWhenDie, originalCol.g + colorValuesIncreaseWhenDie, originalCol.b + colorValuesIncreaseWhenDie);

        mySpriteRenderer.color = lastColor;
        if (upperPartTransform != null)
        {
            Color lastUpperPartColor = new Color(originalUpperPartColor.r + colorValuesIncreaseWhenDie, originalUpperPartColor.g + colorValuesIncreaseWhenDie, originalUpperPartColor.b + colorValuesIncreaseWhenDie);

            upperPartSpriteRenderer.color = lastUpperPartColor;
            upperPartTransform.Find("BrokenSprite").GetComponent<SpriteRenderer>().color = lastUpperPartColor;
            transform.Find("BrokenSprite").GetComponent<SpriteRenderer>().color = lastColor;
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
        pushAwayTime /= 1 + (Time.deltaTime * 3);

        float pushAwayLerpProgress = (-pushAwayTime + 1) * 5;
        if (pushAwayTime < 0.8f) pushAwayLerpProgress = pushAwayTime * 1.111f;

        transform.eulerAngles = new Vector3(0, 0, dead? pushAwayProgress * pushAwayLerpProgress * 3 : pushAwayProgress * pushAwayLerpProgress);
    }
}
