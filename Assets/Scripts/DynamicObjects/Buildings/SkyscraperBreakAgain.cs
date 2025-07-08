using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This entire script is a joke. I did it for no reason at all, it's not necessary for the game but i wanted to do it.

public class SkyscraperBreakAgain : MonoBehaviour, ResetPoolObject
{
    [SerializeField] private ObjectPool middlePartPool, completeDestroyParticlesPool, cutInHalfParticlesPool;
    [SerializeField] private Sprite brokenPartSprite, topSprite;

    private bool imOriginalPart;

    // Variables only meant to reset the state after the object is reused.
    float originalHeight;
    Vector3 upperSpritePos, lowerSpritePos, upperSpriteSize;
    Color upperSpriteOriginalColor;

    private void Awake()
    {
        imOriginalPart = CompareTag("Skyscraper");
        originalHeight = transform.GetComponent<BoxCollider2D>().size.y;
        upperSpritePos = transform.Find("UpperSprite").localPosition;
        if (!imOriginalPart) lowerSpritePos = transform.Find("LowerSprite").localPosition;

        SpriteRenderer upperSpriteRenderer = transform.Find("UpperSprite").GetComponent<SpriteRenderer>();
        upperSpriteOriginalColor = upperSpriteRenderer.color;
        upperSpriteSize = upperSpriteRenderer.size;
    }

    private void Start()
    {
        middlePartPool = GameObject.Find("BuildingPartsContainer/skyscraperMiddlePart").GetComponent<ObjectPool>();
        completeDestroyParticlesPool = GameObject.Find("ParticlesContainer/skyscraperCompletePartDestruction").GetComponent<ObjectPool>();
        cutInHalfParticlesPool = GameObject.Find("ParticlesContainer/skyscraperCutInHalfPieces").GetComponent<ObjectPool>();
    }

    public void BreakAgain(Transform explosionTransform)
    {
        explosionTransform.GetComponent<ExplosionController>().CantBreakSkyscraperAgain = true;

        Vector2 breakPos = GetBreakPos(explosionTransform.position);
        BreakingProcess(breakPos);
    }

    Vector2 GetBreakPos(Vector2 explosionPos)
    {
        float myRotation = transform.eulerAngles.z * Mathf.Deg2Rad;
        float explosionDistanceToCenter = Vector2.Distance(transform.position, explosionPos);
        // If the building part has turned around too much, the explosion position isn't calculated well.
        if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270) explosionDistanceToCenter *= -1;

        // If the explosion is located under the center of the building part, explsionDistance has to be negative for the maths to work out.
        if (transform.position.y > explosionPos.y) explosionDistanceToCenter = -explosionDistanceToCenter;

        // At this point the building part might be rotated, so let's get the closer point to the explosion like this:
        Vector2 breakPos = new Vector2(transform.position.x + Mathf.Sin(myRotation) * -explosionDistanceToCenter, transform.position.y + Mathf.Cos(myRotation) * explosionDistanceToCenter);

        return breakPos;
    }

    void BreakingProcess(Vector2 breakPos) // Once obtained the cut position, the building has to be splitted (or not) AGAIN.
    {
        float myRotation = transform.eulerAngles.z * Mathf.Deg2Rad;
        float myHeightHalf = originalHeight / 2;

        // The positions of both edges. Each one is located at a distance of half building part's height from the center.
        Vector2 lowerEdge = new Vector2(transform.position.x + Mathf.Sin(myRotation) * myHeightHalf, transform.position.y + Mathf.Cos(myRotation) * -myHeightHalf);
        Vector2 upperEdge = new Vector2(transform.position.x + Mathf.Sin(myRotation) * -myHeightHalf, transform.position.y + Mathf.Cos(myRotation) * myHeightHalf);

        // Whether or not the explosion is too close to either upper or lower edge to make the building part be splitted in half.
        bool tooCloseToLowerEdge = Vector2.Distance(breakPos, lowerEdge) < 4;
        bool tooCloseToUpperEdge = Vector2.Distance(breakPos, upperEdge) < 4;

        // If the explosion is too close to any of the edges, the building doesn't split in half, but it is broken where the explosion was.
        if (tooCloseToLowerEdge || tooCloseToUpperEdge)
        {
            if (tooCloseToLowerEdge && tooCloseToUpperEdge)
            {
                DestroyLittleBuildingPiece(gameObject, breakPos);
            }

            if (tooCloseToLowerEdge) // In this case the calculations have to be done comparing the breakingPos to the lower edge.
            {
                Vector2 direction = breakPos - lowerEdge;
                // Prevent the building part from EXPANDING towards the explosionPos in case it is further away than one of the edges.
                if (transform.eulerAngles.z <= 90 && breakPos.y < lowerEdge.y || transform.eulerAngles.z > 90 && breakPos.y > lowerEdge.y) direction = -direction.normalized;

                if (Vector2.Distance(breakPos, lowerEdge) < 1) direction = ((Vector2)transform.position - lowerEdge).normalized;

                DeleteSmallBuildingPart(direction + direction.normalized, false); // Direction increased for a bigger part destroyed.

                // Particles:
                Transform completeDestroyParticlesTransform = completeDestroyParticlesPool.GetObject(true).transform;
                // With the next formula we obtain the center position between the lower edge of the building part and the part where it broke:
                completeDestroyParticlesTransform.position = lowerEdge + direction / 2;
                completeDestroyParticlesTransform.eulerAngles = transform.eulerAngles;

                if (!imOriginalPart)
                {
                    if (transform.eulerAngles.z <= 90)
                    {
                        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 7, ForceMode2D.Impulse);
                    }
                    else
                    {
                        GetComponent<Rigidbody2D>().AddForce(Vector2.down * 3, ForceMode2D.Impulse);
                    }
                }
            }

            else if (tooCloseToUpperEdge) // In this other case the calculations have to be done comparing the breakingPos to the upper edge.
            {
                Vector2 direction = breakPos - upperEdge;
                if (imOriginalPart) direction *= -1; // Why? Wh-why the fuck? I don't know. One day the original part stopped working propely, I did this, now it works. Someone punish me.
                // Prevent the building part from EXPANDING towards the explosionPos in case it is further away than one of the edges.
                if (transform.eulerAngles.z <= 90 && breakPos.y > upperEdge.y || transform.eulerAngles.z > 90 && breakPos.y < upperEdge.y) direction = -direction.normalized;

                if (Vector2.Distance(breakPos, upperEdge) < 1) direction = ((Vector2)transform.position - upperEdge).normalized;

                DeleteSmallBuildingPart(direction + direction.normalized, true); // Direction increased for a bigger part destroyed.

                // Particles
                Transform completeDestroyParticlesTransform = completeDestroyParticlesPool.GetObject(true).transform;
                // With the next formula we obtain the center position between the upper edge of the building part and the part where it broke:
                completeDestroyParticlesTransform.position = upperEdge + direction / 2;
                completeDestroyParticlesTransform.eulerAngles = transform.eulerAngles;

                if (!imOriginalPart)
                {
                    if (transform.eulerAngles.z <= 90)
                    {
                        GetComponent<Rigidbody2D>().AddForce(Vector2.down * 3, ForceMode2D.Impulse);
                    }
                    else
                    {
                        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 7, ForceMode2D.Impulse);
                    }
                }
            }
        }

        else // If the explosion is at a good distance from both edges, the building part can be splitted in half.
        {
            CutInHalf(breakPos, lowerEdge - breakPos, upperEdge - breakPos);

            // Particles
            Transform cutInHalfParticlesTransform = cutInHalfParticlesPool.GetObject(true).transform;
            cutInHalfParticlesTransform.position = new Vector3(breakPos.x, breakPos.y, -2);
            cutInHalfParticlesTransform.eulerAngles = transform.eulerAngles;
        }
    }

    void DestroyLittleBuildingPiece(GameObject buildingPieceToDestroy, Vector2 breakPos)
    {
        // In this remotely possible case, the building piece would be too little that it just needs to be completely annihilated.

        // Particles
        Transform completeDestroyParticlesTransform = completeDestroyParticlesPool.GetObject(true).transform;
        completeDestroyParticlesTransform.position = breakPos;
        completeDestroyParticlesTransform.eulerAngles = transform.eulerAngles;

        Debug.Log("That building didn't deserve it.");
        if (buildingPieceToDestroy.GetComponent<PooledObject>() != null) buildingPieceToDestroy.GetComponent<PooledObject>().ReturnToPool(gameObject);
        else
        {
            Debug.LogWarning("Pooled Object script not found in " + buildingPieceToDestroy.name);
            Destroy(buildingPieceToDestroy);
        }
        return;
    }
    
    // In the case of the explosion being too close to one of the edges, the building doesn't split in half.
    // Instead, the space between the brokenPos and the closer edge is erased, looking like it has been blown up.
    // Direction variable is the direction towards which the building has to move once resized.
    void DeleteSmallBuildingPart(Vector2 direction, bool isUpperPart)
    {
        ResizePart(transform, direction.magnitude, imOriginalPart); // First resize the building to erase the broken part space

        // In case the upper part of the upper piece of the skystraper is broken (the one with the smooth top sprite in the top),
        // the top sprite is changed for a broken sprite.
        SpriteRenderer upperSpriteRenderer = transform.Find("UpperSprite").GetComponent<SpriteRenderer>();
        if (!imOriginalPart && isUpperPart && upperSpriteRenderer.sprite != brokenPartSprite)
        {
            upperSpriteRenderer.sprite = brokenPartSprite;
            upperSpriteRenderer.color = Color.white;
            upperSpriteRenderer.size = new Vector2(4, 2);
        }

        // Then displacing it towards the contrary size of the breakingPos, so that the contrary part of the resized building part
        // to the broken one matches the original one, and the broken part is moved until it matches the explosion pos.
        transform.position += (Vector3)direction;
    }

    // Here, the original part is resized and displaced to be one of the halves, and a new one is created and put in place.
    void CutInHalf(Vector2 cutPos, Vector2 dirToLowerEdge, Vector2 dirToUpperEdge)
    {
        float lowerPartHeight = dirToLowerEdge.magnitude;
        float upperPartHeight = dirToUpperEdge.magnitude;

        // If the building is upside down, the lower part is up, and has to be thrown upwards.
        float forceAddRotationMultiplier = transform.eulerAngles.z > 90 ? -1 : 1;
        // Depending on the height of the building it has to be given more or less force so that it always looks the same.
        float upperPartMultiplierForHeight = (30 - upperPartHeight) / 60; // 30 is the default upper part's size, and then 90 because otherwise it gets too fast.

        Transform newPartTransform = middlePartPool.GetObject(true).transform;
        newPartTransform.position = cutPos;
        newPartTransform.eulerAngles = transform.eulerAngles;
        Rigidbody2D newPartRB = newPartTransform.GetComponent<Rigidbody2D>();

        if (imOriginalPart) // The original part's GameObject structure is different from the other pieces and has no RB.
        {
            // Resizing and positioning the existing part
            ResizePart(transform, upperPartHeight - 1, true); // I always put a -1 so that the two building parts don't overlap.
            transform.position = cutPos + dirToLowerEdge / 2 + dirToLowerEdge.normalized / 2; // "+ dirToUpperEdge.normalized / 2" is to account for the -1.

            // Creating and positioning the new part
            CreateNewPart(newPartTransform, upperPartHeight - 1);
            newPartTransform.position += new Vector3(dirToUpperEdge.x / 2, dirToUpperEdge.y / 2, 0);

            // Adding RB forces to the new part to give space for the player to go through
            newPartRB.AddTorque(600 * upperPartMultiplierForHeight);
            newPartRB.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
        }
        else
        {
            // Resizing and positioning the existing part
            ResizePart(transform, lowerPartHeight - 1, false); // -1 is to leave a little space in the first frame when it is splitted.
            transform.position = cutPos + dirToUpperEdge / 2 + dirToUpperEdge.normalized / 2; // "+ dirToUpperEdge.normalized / 2" is to account for the -1.

            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            // Adding RB forces to give space for the player to go through.
            rb.velocity = Vector2.zero;
            rb.AddTorque(600 * upperPartMultiplierForHeight);
            rb.AddForce(Vector2.up * 7 * forceAddRotationMultiplier, ForceMode2D.Impulse);

            // Creating and positioning the new part (just the same we did with the existing part but inverted)
            if (transform.name.Contains("Middle")) // For some reason, if the middle part instantiates ANOTHER middle part, it isn't created properly, sizes don't fit.
            {
                DestroyLittleBuildingPiece(newPartTransform.gameObject, transform.position + new Vector3(dirToLowerEdge.x / 2, dirToLowerEdge.y / 2, 1));
            }
            else
            {
                CreateNewPart(newPartTransform, lowerPartHeight - 1);
                newPartTransform.position += new Vector3(dirToLowerEdge.x / 2, dirToLowerEdge.y / 2, 0);

                float lowerPartMultiplierForHeight = (30 - lowerPartHeight) / 60; // Same as upperPartMultiplierForHeight
                newPartRB.AddTorque(-600 * lowerPartMultiplierForHeight);
                newPartRB.velocity = rb.velocity;
            }
        }
    }

    void ResizePart(Transform partTransform, float sizeSubstracter, bool isOriginalPart)
    {
        // Imagine a building part with height 5. if the cut happens in height 3, the resting height is 2,
        // so the part's height is decreased by 2 (the height of the remaining part), the upper part is descended 1 unit
        // (the resting height / 2), and the lower part is heightened 1 unit, to match the two corners.

        Vector2 colliderSize = partTransform.GetComponent<BoxCollider2D>().size;
        Vector2 newSize = new Vector2(colliderSize.x, colliderSize.y - sizeSubstracter);
        // If this was the example, colliderSize.y is 5, and sizeSubstracter is 2, leaving a heigth of 3

        if (newSize.y < 3)
        {
            DestroyLittleBuildingPiece(partTransform.gameObject, partTransform.position);
            return;
        }

        if (isOriginalPart) // The original part's GameObject structure is different from the other pieces.
        {
            partTransform.GetComponent<BoxCollider2D>().size = newSize;
            partTransform.Find("Sprite").GetComponent<SpriteRenderer>().size = newSize;
            partTransform.Find("BackSprite").GetComponent<SpriteRenderer>().size = newSize;

            partTransform.Find("UpperSprite").localPosition -= new Vector3(0, sizeSubstracter / 2, 0);
            // If this was the example, sizeSubstracter is 2 and UpperSprite is lowered 1 unit, just over the main sprite.
        }
        else
        {
            partTransform.GetComponent<BoxCollider2D>().size = newSize;
            partTransform.GetComponent<SpriteRenderer>().size = newSize;

            partTransform.Find("UpperSprite").localPosition -= new Vector3(0, sizeSubstracter / 2, 0);
            partTransform.Find("LowerSprite").localPosition += new Vector3(0, sizeSubstracter / 2, 0);
        }
    }

    void CreateNewPart(Transform partTransform, float partSize)
    {
        Vector2 newSize = new Vector2(partTransform.GetComponent<BoxCollider2D>().size.x, partSize);

        partTransform.GetComponent<BoxCollider2D>().size = newSize;
        partTransform.GetComponent<SpriteRenderer>().size = newSize;

        partTransform.Find("UpperSprite").localPosition += new Vector3(0, partSize / 2, 0);
        partTransform.Find("LowerSprite").localPosition -= new Vector3(0, partSize / 2, 0);
    }


    // Reset Pooled Object State

    public void ResetState()
    {
        if (imOriginalPart)
        {
            GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<BoxCollider2D>().size.x, originalHeight);
            transform.Find("Sprite").GetComponent<SpriteRenderer>().size = new Vector2(transform.Find("Sprite").GetComponent<SpriteRenderer>().size.x, originalHeight);
            transform.Find("BackSprite").GetComponent<SpriteRenderer>().size = new Vector2(transform.Find("BackSprite").GetComponent<SpriteRenderer>().size.x, originalHeight);
        }
        else
        {
            GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<BoxCollider2D>().size.x, originalHeight);
            GetComponent<SpriteRenderer>().size = new Vector2(GetComponent<SpriteRenderer>().size.x, originalHeight);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.rotation = 0f;
        }

        transform.eulerAngles = Vector3.zero;
        transform.Find("UpperSprite").localPosition = upperSpritePos;
        if (!imOriginalPart) transform.Find("LowerSprite").localPosition = lowerSpritePos;

        SpriteRenderer upperSpriteRenderer = transform.Find("UpperSprite").GetComponent<SpriteRenderer>();
        if (transform.name.Contains("Upper")) upperSpriteRenderer.sprite = topSprite;
        upperSpriteRenderer.color = upperSpriteOriginalColor;
        upperSpriteRenderer.size = upperSpriteSize;
    }

    public void Initialize()
    {

    }
}
