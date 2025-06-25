using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneController : MonoBehaviour
{
    [SerializeField] private Color burnColor, possibleColor1, possibleColor2, possibleColor3;

    [SerializeField] private float moveSpeed, rotFactor, playerRotationSensitivity, pushForce;
    public float actualRotationSpeed;
    private float maxXdistance, rotationWhenDutyFinished, stabilizerLerp, rotationToAddWhenClose, XdistanceWhenEnteredCloseRange, timeAfterDutyFinished = 1,
                  rotSpeedWhenFinished, residualRotation, dyingXpos, appearingXpos, playerXpos, boostSoundVolume;

    public bool dead, dutyFinished, enteredCloseRange;

    private Transform playerTransform;

    private InGameSound boostSoundOptionsModifier;

    private Rigidbody2D rb;

    void Start()
    {
        if (PlayerController.dead) dutyFinished = true;
        else playerTransform = GameObject.Find("Player").transform;

        rb = transform.GetComponent<Rigidbody2D>();
        boostSoundOptionsModifier = transform.Find("Boost/BoostSound").GetComponent<InGameSound>();

        //ChoseColor();

        dyingXpos = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 10;
        playerXpos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 5, 0)).x;
        maxXdistance = transform.position.x - playerXpos;
        transform.position = new Vector3(transform.position.x, Random.Range(Camera.main.ScreenToWorldPoint(Vector3.zero).y, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y), transform.position.z);

        boostSoundVolume = boostSoundOptionsModifier.gameObject.GetComponent<AudioSource>().volume;
        appearingXpos = transform.position.x;
    }

    void ChoseColor()
    {
        Color chosenColor = possibleColor1;
        float randValue = Random.value;
        if (randValue > 0.66f) chosenColor = possibleColor2;
        else if (randValue > 0.33f) chosenColor = possibleColor3;

        for (int childNum = 0; childNum < transform.Find("Parts").childCount; childNum++) transform.Find("Parts").GetChild(childNum).GetComponent<SpriteRenderer>().color = chosenColor;
    }

    void Update()
    {
        if (!dead) RotateAndMove();

        if (transform.position.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y - 5 || transform.position.y > Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y + 5) Destroy(gameObject);

        if (transform.position.x > playerXpos && !InGameSound.threeDsound) BoostSoundAppear();
        if (transform.position.x < playerXpos) BoostSoundDecay();
    }

    void RotateAndMove()
    {
        float lastFrameRot = transform.eulerAngles.z;

        if (!dutyFinished && (transform.position.x - playerTransform.position.x < 1 || PlayerController.dead))
        {
            dutyFinished = true;
            rotationWhenDutyFinished = transform.eulerAngles.z;
            rotSpeedWhenFinished = actualRotationSpeed * Time.deltaTime;
        }

        if (!dutyFinished) TowardsPlayer();
        else if (Time.timeScale >= 0.5f) AfterPlayer();

        actualRotationSpeed = (transform.eulerAngles.z - lastFrameRot) / Time.deltaTime;

        transform.position += new Vector3(0, -(transform.eulerAngles.z - 90) * Time.deltaTime * moveSpeed * ObjectPassingBy.realSpeedMultiplier, 0); // Move in Y in function of plane's rotation.
    }

    void TowardsPlayer()
    {
        Vector2 distToPlayer = new Vector2(transform.position.x - playerTransform.position.x, transform.position.y - playerTransform.position.y);

        float playerRotationFactor = Mathf.Cos((playerTransform.eulerAngles.z - 180) / 57.3f) * 0.5f + 0.5f; // The more inclinated player is, the higher this variable will be.

        float rotationAdder = 
            rotFactor * distToPlayer.y * (-distToPlayer.x / maxXdistance + 1) - //When closer in X to the player, stronger the rotation towards it will be.
            playerRotationFactor * playerRotationSensitivity; // Have in mind player's rotation to go upper if the player does so.

        //if ((transform.eulerAngles.z > 40 && rotationAdder < 0) || (transform.eulerAngles.z < 140 && rotationAdder > 0)) transform.eulerAngles += new Vector3(0, 0, rotationAdder);

        if (distToPlayer.x < 7)
        {
            if (!enteredCloseRange)
            {
                enteredCloseRange = true;
                XdistanceWhenEnteredCloseRange = distToPlayer.x;
                // This variable is needed to make the effect 0 when the close range effect starts, and make it stronger the closer it gets.
            }
            rotationToAddWhenClose += rotFactor * distToPlayer.y * (-distToPlayer.x / XdistanceWhenEnteredCloseRange + 1) * Time.deltaTime * 4;
        }
        // RotationToAddWhenClose is used to make the plane approach more the player when getting to it, as many times it doesn't catch it with the normal rotation used.

        transform.eulerAngles = new Vector3(0, 0, Mathf.Clamp(rotationAdder + 90 + rotationToAddWhenClose, 30, 150));
    }

    void AfterPlayer() // After the player dies or goes out of reach, the plane smoothly stabilizes to an horizontal direction.
    {
        timeAfterDutyFinished += Time.deltaTime * 3;
        residualRotation += rotSpeedWhenFinished / timeAfterDutyFinished; // Mantain the rotation the plane had when it finished duty.
        float clampedTimeAfterDutyFinished = Mathf.Clamp01(timeAfterDutyFinished - 1); // At the beggining, start slower to avoid a forced change in direction.

        stabilizerLerp += Time.deltaTime * (1 - stabilizerLerp) * clampedTimeAfterDutyFinished; // The more time has passed since duty finished, the more normalized its direction will be.
        
        transform.eulerAngles = new Vector3(0, 0, Mathf.Clamp(Mathf.Lerp(rotationWhenDutyFinished, 90, stabilizerLerp) + residualRotation * (1 - stabilizerLerp), 10, 170));
        // Clamped so that it doesn't rotate too much. Residual rotation is added multiplied by stabilizerLerp inversed, so that residualRotation has less effect with time.
    }

    void BoostSoundAppear() // Make the Boost Sound disappear smoothly, instead of a hard cut when the plane disappears once out of camera.
    {
        float appearingProgress = ((transform.position.x - playerXpos) / (playerXpos - appearingXpos) + 1) * boostSoundVolume;
        // Gives the index of remoteness from the player since the plane appears (from 1 to 0). 1 is the exact X player position, 0 is the enemy's appearing
        // X position. Then it's multiplied by the boost sound volume so that the volume doesn't start increasing until 1 regardless the default volume.

        boostSoundOptionsModifier.SetRealVolume = appearingProgress;
        // Then, the boost sound volume is set to the decay progress (from 0.25 to 0) progressively, so that the sound disappears smoothly.
    }

    void BoostSoundDecay() // Make the Boost Sound disappear smoothly, instead of a hard cut when the plane disappears once out of camera.
    {
        float decayProgress = ((playerXpos - transform.position.x) / (dyingXpos - playerXpos) + 1) * boostSoundVolume;
        // Gives the index of remoteness from the player once it has passed by the enemy plane (from 1 to 0). 1 is the exact X player position, 0 is the enemy's disappearing
        // X position (defined by ObjectPassingBy). Then it's multiplied by the boost sound volume so that the volume doesn't start decreasing from 1 regardless the default volume.

        boostSoundOptionsModifier.SetRealVolume = decayProgress;
        // Then, the boost sound volume is set to the decay progress (from 0.25 to 0) progressively, so that the sound disappears smoothly.
    }

    public void Die()
    {
        if (dead) return;
        dead = true;

        rb.angularDrag = 0;
        rb.gravityScale = 2;

        rb.AddTorque(Random.Range(80f, 240f));
        if (Random.value > 0.5f) rb.angularVelocity *= -1;
        rb.AddForce(new Vector2(Random.Range(-2, 3), 5), ForceMode2D.Impulse);

        //for (int childNum = 0; childNum < transform.Find("Parts").childCount; childNum++) transform.Find("Parts").GetChild(childNum).GetComponent<SpriteRenderer>().color = burnColor;

        transform.GetComponent<Collider2D>().enabled = false;
        transform.GetComponent<ObjectPassingBy>().enabled = false;
        transform.Find("Boost").gameObject.SetActive(false);
        //transform.Find("Lights").gameObject.SetActive(false);
    }
}
