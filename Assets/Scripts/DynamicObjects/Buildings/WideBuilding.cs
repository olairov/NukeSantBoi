using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WideBuilding : Building, ResetPoolObject
{
    [SerializeField] private Sprite buildingSprite2, buildingSprite3, backBuildingSprite2, backBuildingSprite3;
    static int lastSprite;
    ObjectPool defaultBuildingPool;

    private Material instanceMaterial, backSpriteInstanceMaterial;
    private SpriteRenderer myRenderer;

    [SerializeField] List<Sprite> mainSprites = new(), backSprites = new();
    [SerializeField] List<Vector2> colliderOffsetsForEachSprite = new();

    Vector2 defaultColliderSize, defaultColliderOffset;

    protected override void Start()
    {
        base.Start();

        defaultBuildingPool = GameObject.Find("BuildingGenerator/DefaultBuildingGenerator").GetComponent<ObjectPool>();

        myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (transform.name.Contains("Sky")) iStrapSky = true;

        defaultColliderSize = GetComponent<BoxCollider2D>().size;
        defaultColliderOffset = GetComponent<BoxCollider2D>().offset;

        Initialize();
    }

    protected override void Update()
    {
        base.Update();
    }

    void ChooseStats()
    {
        float randomY = Random.Range(0f, 1.5f);

        if (Random.value > 0.999f)
        {
            defaultBuildingPool.GetObject(true).GetComponent<Building>().imUpsideDown = true;
        }

        float myYdefaultPos = Camera.main.ScreenToWorldPoint(Vector3.zero).y + randomY;
        transform.position = new Vector3(transform.position.x, myYdefaultPos, -0.5f);

        int chosenBuildingSpriteIdx = ChooseBuildingSprite(mainSprites.Count, lastSprite);

        SpriteRenderer frontSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SpriteRenderer backSprite = myBackSprite.GetComponent<SpriteRenderer>();
        BoxCollider2D myCollider = GetComponent<BoxCollider2D>();

        frontSprite.sprite = mainSprites[chosenBuildingSpriteIdx];
        backSprite.sprite = backSprites[chosenBuildingSpriteIdx];
        myCollider.offset = colliderOffsetsForEachSprite[chosenBuildingSpriteIdx];

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
        
        posToPutDecal = new Vector2(Mathf.Clamp(posToPutDecal.x, -1, 1), posToPutDecal.y);

        if (posToPutDecal.y < -2.4f) posToPutDecal = new Vector2(posToPutDecal.x, -2.4f);
        
        instanceMaterial.SetVector("_breakPos", posToPutDecal);
        backSpriteInstanceMaterial.SetVector("_breakPos", posToPutDecal);
    }


    // Reset Pooled Object State

    public override void ResetState()
    {
        base.ResetState();

        BoxCollider2D myColider = GetComponent<BoxCollider2D>();
        myColider.offset = defaultColliderOffset;
        myColider.size = defaultColliderSize;

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
        
        ChooseStats();
    }
}
