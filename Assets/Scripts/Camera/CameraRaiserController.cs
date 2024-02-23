using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaiserController : MonoBehaviour
{
    private Transform playerTransform, deadLineTransform;

    private SpriteRenderer deadLineSprite;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        deadLineTransform = GameObject.Find("Borders/DeadLine").transform;

        deadLineSprite = deadLineTransform.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        ChangePos();

        if (PlayerController.dead) transform.parent.GetComponent<Animator>().SetTrigger("Crash");
    }

    private void ChangePos()
    {
        if (PlayerController.dead) return;

        float posLerp = Mathf.Pow(playerTransform.position.y, 5) * 0.0000305f;
        posLerp = Mathf.Clamp01(posLerp) / 4;

        transform.position = new Vector3(0, playerTransform.position.y / 60 + posLerp, transform.position.z);

        ChangeDeathLineStats();
    }

    private void ChangeDeathLineStats()
    {
        float statsLerp = playerTransform.position.y - 5f;
        statsLerp = Mathf.Clamp01(statsLerp);

        deadLineTransform.localScale = new Vector3(deadLineTransform.localScale.x, Mathf.Lerp(1f, 1.8f, statsLerp), deadLineTransform.localScale.z);
        deadLineSprite.color = new Color(deadLineSprite.color.r, deadLineSprite.color.g, deadLineSprite.color.b, statsLerp / 1.5f);
    }
}
