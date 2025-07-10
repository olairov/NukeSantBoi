using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WideBuilding : Building, ResetPoolObject
{
    [SerializeField] private Sprite buildingSprite2, buildingSprite3, backBuildingSprite2, backBuildingSprite3;
    static Sprite lastDefaultSprite, lastWideSprite;
    ObjectPool defaultBuildingPool;

    private Material instanceMaterial, backSpriteInstanceMaterial;
    private SpriteRenderer myRenderer;

    private float myYdefaultPos;

    private bool imWide;

    protected override void Start()
    {
        base.Start();

        defaultBuildingPool = GameObject.Find("BuildingGenerator/DefaultBuildingGenerator").GetComponent<ObjectPool>();

        myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (transform.name.Contains("Sky")) iStrapSky = true;

        Initialize();

        ChooseStats();
    }

    protected override void Update()
    {
        base.Update();
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
            defaultBuildingPool.GetObject(true).GetComponent<Building>().imUpsideDown = true;
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

    public override void Destroy(Transform otherTransform)
    {
        base.Destroy(otherTransform);

        SetGap(otherTransform.position);
    }

    IEnumerator SetGap(Vector2 otherPos)
    {
        yield return new WaitForSeconds(flashDuration);

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
        posToPutDecal = transform.position;
        instanceMaterial.SetVector("_breakPos", posToPutDecal);
        backSpriteInstanceMaterial.SetVector("_breakPos", posToPutDecal);
    }


    // Reset Pooled Object State

    public override void ResetState()
    {
        base.ResetState();

        myYdefaultPos = 0;
        imUpsideDown = false;
    }

    public override void Initialize()
    {
        base.Initialize();

        // Set the material for the gap shader.
        instanceMaterial = new Material(myRenderer.sharedMaterial);
        myRenderer.material = instanceMaterial;
        instanceMaterial.SetVector("_breakPos", new Vector2(0, 3));

        // Set the material for the gap shader to the back sprite.
        backSpriteInstanceMaterial = new Material(myBackSprite.GetComponent<SpriteRenderer>().sharedMaterial);
        myBackSprite.GetComponent<SpriteRenderer>().material = backSpriteInstanceMaterial;
        backSpriteInstanceMaterial.SetVector("_breakPos", new Vector2(0, 3));

        // Set the material for the gap shader accordingly to being wide.
        instanceMaterial.SetFloat("_tiling", 4);
        backSpriteInstanceMaterial.SetFloat("_tiling", 4);
        
        imWide = true;

        ChooseStats();
    }
}
