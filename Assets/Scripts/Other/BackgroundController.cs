using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private int zPos;

    [SerializeField] private Sprite background1, background2, background3;
    private Sprite lastBackground;

    public Sprite LastBackground
    {
        set { lastBackground = value; }
    }

    void Start()
    {
        CreateStats();
    }

    void CreateStats()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zPos);

        Sprite mySprite = background1;
        while (true)
        {
            float randomNum = Random.value;
            if (randomNum > 0.66f) mySprite = background2;
            else if (randomNum > 0.33f) mySprite = background3;

            if (lastBackground != mySprite) break;
        }

        GetComponent<SpriteRenderer>().sprite = mySprite;
        if (name.Contains("1")) GameObject.Find("__________________Map___________________").GetComponent<MapGenerator>().lastBackgroundLayer1 = mySprite;
        else if (name.Contains("2")) GameObject.Find("__________________Map___________________").GetComponent<MapGenerator>().lastBackgroundLayer2 = mySprite;
        else GameObject.Find("__________________Map___________________").GetComponent<MapGenerator>().lastBackgroundLayer3 = mySprite;
    }
}
