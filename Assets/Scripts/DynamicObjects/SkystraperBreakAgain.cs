using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This entire script is a joke. I did it for no reason at all, it's not necessary for the game but i wanted to do it.
// Enjoy doing whatever you're doing here, but if your intention is understanding it, i wish you the best of luck.

public class SkystraperBreakAgain : MonoBehaviour
{
    [SerializeField] private GameObject middlePartPrefab, completeDestroyParticlesPref, cutInHalfParticlesPref;
    [SerializeField] private Sprite brokenPartSprite;

    private Transform particlesContainer;

    private bool alreadyBroken, oneFramePassedSinceBreakAgainCall, imOriginalPart;
    public bool SetAlreadyBroken
    {
        set { alreadyBroken = value; }
    }

    private void Start()
    {
        imOriginalPart = CompareTag("Skystraper");
        particlesContainer = GameObject.Find("ParticlesContainer").transform;

        if (name.Contains("SkystraperMiddlePart")) alreadyBroken = true;
    }

    public void BreakAgain(Vector2 explosionPos)
    {
        if (!oneFramePassedSinceBreakAgainCall)
        {
            // This is needed to ensure this is not executed the first time the building is broken.
            oneFramePassedSinceBreakAgainCall = true;
            return;
        }
        else if (!alreadyBroken) return;

        Vector2 breakPos = GetBreakPos(explosionPos);
        BreakingProcess(breakPos);
    }

    Vector2 GetBreakPos(Vector2 explosionPos)
    {
        float myRotation = transform.eulerAngles.z * Mathf.Deg2Rad;
        float explosionDistanceToCenter = Vector2.Distance(transform.position, explosionPos);

        // If the explosion is located under the center of the building part, explsionDistance has to be negative for the maths to work out.
        if (transform.position.y > explosionPos.y) explosionDistanceToCenter = -explosionDistanceToCenter;

        // At this point the building part might be rotated, so let's get the closer point to the explosion like this:
        Vector2 breakPos = new Vector2(transform.position.x + Mathf.Sin(myRotation) * -explosionDistanceToCenter, transform.position.y + Mathf.Cos(myRotation) * explosionDistanceToCenter);

        return breakPos;
    }

    void BreakingProcess(Vector2 breakPos) // Once obtained the cut position, the building has to be splitted (or not) AGAIN.
    {
        float myRotation = transform.eulerAngles.z * Mathf.Deg2Rad;
        float myHeightHalf = transform.GetComponent<BoxCollider2D>().size.y / 2;

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
                DestroyLittleBuildingPiece(gameObject ,breakPos);
            }

            if (!imOriginalPart) GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Prepare the building to add it force upwards.
            if (tooCloseToLowerEdge) // In this case the calculations have to be done comparing the breakingPos to the lower edge.
            {
                Vector2 direction = breakPos - lowerEdge;
                DeleteSmallBuildingPart(direction + direction.normalized * 1.5f, false); // Direction increased for a bigger part destroyed.
                /* Particles */ Instantiate(completeDestroyParticlesPref, breakPos - direction.normalized, Quaternion.identity, particlesContainer);

                // This impulses the building part away from the explosion. All these multiplier's why are explained in the CutInHalf function.
                float multiplierForHeight = Vector2.Distance(breakPos, upperEdge) / (Vector2.Distance(lowerEdge, upperEdge));
                if (!imOriginalPart) GetComponent<Rigidbody2D>().AddForce(Vector3.up * 7 * (myRotation > 90 ? -1 : 1) * multiplierForHeight, ForceMode2D.Impulse);
            }
            else // In this other case the calculations have to be done comparing the breakingPos to the upper edge.
            {
                Vector2 direction = breakPos - upperEdge;
                DeleteSmallBuildingPart(direction + direction.normalized * 1.5f, true); // Direction increased for a bigger part destroyed.
                /* Particles */ Instantiate(completeDestroyParticlesPref, breakPos - direction.normalized, Quaternion.identity, particlesContainer);

                // This impulses the building part away from the explosion. All these multiplier's why are explained in the CutInHalf function.
                float multiplierForHeight = Vector2.Distance(breakPos, lowerEdge) / (Vector2.Distance(lowerEdge, upperEdge));
                if (!imOriginalPart) GetComponent<Rigidbody2D>().AddForce(Vector3.down * 7 * (myRotation > 90 ? -1 : 1) * multiplierForHeight, ForceMode2D.Impulse);
            }
        }
        else // If the explosion is at a good distance from both edges, the building part can be splitted in half.
        {
            CutInHalf(breakPos, lowerEdge - breakPos, upperEdge - breakPos);
            /* Particles */ Instantiate(cutInHalfParticlesPref, new Vector3(breakPos.x, breakPos.y, -2), Quaternion.identity, particlesContainer);
        }
    }

    void DestroyLittleBuildingPiece(GameObject buildingPieceToDestroy, Vector2 breakPos)
    {
        // In this remotely possible case, the building piece would be too little that it just needs to be completely annihilated.
        /* Particles */
        Instantiate(completeDestroyParticlesPref, breakPos, Quaternion.identity, particlesContainer);
        Debug.Log("That building didn't deserve it.");
        Destroy(buildingPieceToDestroy);
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
        float forceAddMultiplier = transform.eulerAngles.z > 90 ? -1 : 1;
        // Depending on the height of the building it has to be given more or less force so that it always looks the same.
        float upperPartMultiplierForHeight = upperPartHeight / (upperPartHeight + lowerPartHeight);

        Transform newPartTransform = Instantiate(middlePartPrefab, cutPos, Quaternion.identity, transform.parent).transform;
        Rigidbody2D newPartRB = newPartTransform.GetComponent<Rigidbody2D>();

        if (imOriginalPart) // The original part's GameObject structure is different from the other pieces and has no RB.
        {
            // Resizing and positioning the existing part
            ResizePart(transform, upperPartHeight - 1, true); // I always put a -1 so that the two building parts don't overlap.
            transform.position = cutPos + dirToLowerEdge / 2 + dirToLowerEdge.normalized / 2;

            // Creating and positioning the new part
            CreateNewPart(newPartTransform, upperPartHeight - 1);
            newPartTransform.position += new Vector3(dirToUpperEdge.x / 2, dirToUpperEdge.y / 2, 1);
            newPartTransform.eulerAngles = transform.eulerAngles;

            // Adding RB forces to the new part to give space for the player to go through
            newPartRB.AddTorque(600 * upperPartMultiplierForHeight);
            newPartRB.AddForce(Vector3.up * 7 * forceAddMultiplier * upperPartMultiplierForHeight, ForceMode2D.Impulse);
        }
        else
        {
            // Resizing and positioning the existing part
            ResizePart(transform, lowerPartHeight - 1, false);
            transform.position = cutPos + dirToUpperEdge / 2 + dirToUpperEdge.normalized / 2;

            // Creating and positioning the new part
            CreateNewPart(newPartTransform, lowerPartHeight - 1);
            newPartTransform.position += new Vector3(dirToLowerEdge.x / 2, dirToLowerEdge.y / 2, 1);
            newPartTransform.eulerAngles = transform.eulerAngles;

            float lowerPartMultiplierForHeight = lowerPartHeight / (upperPartHeight + lowerPartHeight); // Same as upperPartMultiplierForHeight

            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            // Adding RB forces to the new part and the existing one to give space for the player to go through.
            newPartRB.AddTorque(-600 * lowerPartMultiplierForHeight);
            newPartRB.AddForce(Vector3.down * 7 * forceAddMultiplier * lowerPartMultiplierForHeight, ForceMode2D.Impulse);
            rb.velocity = Vector2.zero;
            rb.AddTorque(600 * upperPartMultiplierForHeight);
            rb.AddForce(Vector3.up * 7 * forceAddMultiplier * upperPartMultiplierForHeight, ForceMode2D.Impulse);
        }
    }

    void ResizePart(Transform partTransform, float sizeSubstracter, bool isOriginalPart)
    {
        // Imagine a building part with height 5. if the cut happens in height 3, the resting height is 2,
        // so the part's height is decreased by 2 (the height of the remaining part), the upper part is descended 1 unit
        // and the lower part is highthened 1 unit (the reesting height / 2), to match the two corners.

        Vector2 colliderSize = partTransform.GetComponent<BoxCollider2D>().size;
        Vector2 newSize = new Vector2(colliderSize.x, colliderSize.y - sizeSubstracter);
        // If this was the example, colliderSize.y is 5, and sizeSubstracter is 2, leaving a heigth of 3

        if (newSize.y < 3) DestroyLittleBuildingPiece(partTransform.gameObject ,partTransform.position);

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
}
