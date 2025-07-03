using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private int zPos;

    [SerializeField] private Sprite background1, background2, background3;
    Sprite lastBackgroundLayer1, lastBackgroundLayer2, lastBackgroundLayer3;

    void Start()
    {
        CreateStats();
    }

    void CreateStats()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zPos);

        Sprite mySprite = background1;
        for (int idx = 0; idx < 100; idx++) // A 100 times loop to avoid excessive repetition if for some reason the sprite always matches the last sprite.
        {
            float randomNum = Random.value;
            if (randomNum > 0.66f) mySprite = background2;
            else if (randomNum > 0.33f) mySprite = background3;

            if (name.Contains("1") && CheckBackgroundMismatch(mySprite, lastBackgroundLayer1))
            {
                lastBackgroundLayer1 = mySprite;
                break;
            }
            else if (name.Contains("2") && CheckBackgroundMismatch(mySprite, lastBackgroundLayer2))
            {
                lastBackgroundLayer2 = mySprite;
                break;
            }
            else if (CheckBackgroundMismatch(mySprite, lastBackgroundLayer3))
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

    bool CheckBackgroundMismatch(Sprite sprite, Sprite lastSprite)
    {
        return sprite != lastSprite;
    }
}
