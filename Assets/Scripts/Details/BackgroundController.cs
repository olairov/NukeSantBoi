using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour, ResetPoolObject
{
    [SerializeField] private int zPos;

    [SerializeField] private Sprite background1, background2, background3;
    static Sprite lastBackgroundLayer1, lastBackgroundLayer2, lastBackgroundLayer3;

    void Start()
    {
        Initialize();
    }


    // Reset Pooled Object State

    public void ResetState()
    {
        
    }

    public void Initialize()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zPos);

        Sprite mySprite = background1;
        for (int idx = 0; idx < 100; idx++) // A 100 times loop to avoid excessive repetition if for some reason the sprite always matches the last sprite.
        {
            float randomNum = Random.value;
            if (randomNum > 0.66f) mySprite = background2;
            else if (randomNum > 0.33f) mySprite = background3;

            if (name.Contains("1") && mySprite != lastBackgroundLayer1)
            {
                lastBackgroundLayer1 = mySprite;
                break;
            }
            else if (name.Contains("2") && mySprite != lastBackgroundLayer2)
            {
                lastBackgroundLayer2 = mySprite;
                break;
            }
            else if (mySprite != lastBackgroundLayer3)
            {
                lastBackgroundLayer3 = mySprite;
                break;
            }

            if (idx >= 99)
            {
                mySprite = background1;
                lastBackgroundLayer1 = mySprite;
                break;
            }
        }

        GetComponent<SpriteRenderer>().sprite = mySprite;
    }
}
