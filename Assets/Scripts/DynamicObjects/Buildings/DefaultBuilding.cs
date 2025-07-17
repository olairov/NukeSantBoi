using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBuilding : Building, ResetPoolObject
{
    [SerializeField] private Sprite buildingSprite2, buildingSprite3, backBuildingSprite2, backBuildingSprite3;
    static int lastSprite;

    private Material instanceMaterial, backSpriteInstanceMaterial;
    private SpriteRenderer myRenderer;

    [SerializeField] List<Sprite> mainSprites = new(), backSprites = new();
    [SerializeField] List<Vector2> spriteSizesForEachSprite = new();
    
    protected override void Start()
    {
        base.Start();

        myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        Initialize();
    }

    protected override void Update()
    {
        base.Update();
    }

    void ChooseStats()
    {
        float randomY = Random.Range(0f, 2.3f);

        if (imUpsideDown)
        {
            randomY = 14;
            transform.localScale = new Vector3(transform.localScale.x, -Mathf.Abs(transform.localScale.y), transform.localScale.z);
        }

        float myYdefaultPos = Camera.main.ScreenToWorldPoint(Vector3.zero).y + randomY;
        transform.position = new Vector3(imUpsideDown ? transform.position.x + 4 : transform.position.x, myYdefaultPos, -0.5f);

        int chosenBuildingSpriteIdx = ChooseBuildingSprite(mainSprites.Count, lastSprite);

        SpriteRenderer frontSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SpriteRenderer backSprite = myBackSprite.GetComponent<SpriteRenderer>();

        frontSprite.sprite = mainSprites[chosenBuildingSpriteIdx];
        backSprite.sprite = backSprites[chosenBuildingSpriteIdx];
        frontSprite.size = spriteSizesForEachSprite[chosenBuildingSpriteIdx];
        backSprite.size = spriteSizesForEachSprite[chosenBuildingSpriteIdx];

        lastSprite = chosenBuildingSpriteIdx;
    }

    public override void Destroy(Transform otherTransform)
    {
        base.Destroy(otherTransform);

        StartCoroutine(SetGap(otherTransform.position));
    }

    IEnumerator SetGap(Vector2 otherPos)
    {
        yield return new WaitForSeconds(0.1f);

        Vector2 posToPutDecal = ((Vector2)transform.position - otherPos) / Mathf.PI - new Vector2(0, 1);

        posToPutDecal = new Vector2(Mathf.Clamp(posToPutDecal.x, -0.4f, 0.4f), posToPutDecal.y);

        if (myRenderer.sprite == buildingSprite3)
        {
            if (posToPutDecal.y < -2) posToPutDecal = new Vector2(posToPutDecal.x, -2f);
        }
        else
        {
            if (posToPutDecal.y < -2.3f) posToPutDecal = new Vector2(posToPutDecal.x, -2.3f);
        }

        instanceMaterial.SetVector("_breakPos", posToPutDecal);
        backSpriteInstanceMaterial.SetVector("_breakPos", posToPutDecal);
    }


    // Reset Pooled Object State

    public override void ResetState()
    {
        base.ResetState();

        transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
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

        ChooseStats();
    }
}
