using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaiserController : MonoBehaviour
{
    private PlayerController playerScript;
    private ShakeController myShakeScript;
    private Transform playerTransform, deadLineTransform;

    private SpriteRenderer deadLineSprite;

    [SerializeField] private float lowerLimit, speedDecreaseInterval, movementAmount;
    // movementAmount is a value from 0 to 1.  0 = Any movement.  1 = Full player follow.
    // The limit is the PLAYER's Ypos in world coordinates where the camera has already stopped moving completely.
    // speedDecreaseInterval + lowerLimit is the PLAYER's pos where the camera will start decreasing movement.

    private float myYpos;

    private bool hasCrashed;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        playerScript = playerTransform.GetComponent<PlayerController>();
        deadLineTransform = GameObject.Find("Borders/DeadLine").transform;
        myShakeScript = GetComponent<ShakeController>();

        deadLineSprite = deadLineTransform.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        ChangePos();

        if (PlayerController.dead && !hasCrashed)
        {
            transform.parent.GetComponent<Animator>().SetTrigger("Crash");
            hasCrashed = true;
        }
    }

    private void ChangePos()
    {
        if (PlayerController.dead) return;

        float limitMultiplier = Mathf.Clamp01((playerTransform.position.y - lowerLimit) / speedDecreaseInterval);
        myYpos += playerScript.GetPlayerYvelocity * Time.deltaTime * movementAmount * limitMultiplier;

        // It sometimes gives an error saying myYpos is a weird thing that doesn't fit in the localPosition.
        if (myYpos < 100000) myShakeScript.SetYdisplacement = myYpos;

        ChangeDeathLineStats();
    }

    private void ChangeDeathLineStats()
    {
        // lineLerp contains a value from 0 to 1 depending on the player distance to the screen top limit.
        // 2 units of distance below it or less is 0, one unit of distance from screen top limit or more is 1.

        // if the Ypos of the player was 3.5 and the Ypos of the screen top was 5:  3.5 - (5 - 1) + 1  =  -0.5 + 1  =  0.5.
        float lineLerp = playerTransform.position.y - (Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - 1) + 1;
        lineLerp = Mathf.Clamp01(lineLerp);

        deadLineTransform.localScale = new Vector3(deadLineTransform.localScale.x, Mathf.Lerp(1f, 2f, lineLerp), deadLineTransform.localScale.z);
        deadLineSprite.color = new Color(deadLineSprite.color.r, deadLineSprite.color.g, deadLineSprite.color.b, lineLerp * 0.8f);
    }
}
