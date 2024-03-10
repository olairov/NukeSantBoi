using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab, explosionPrefab;
    [SerializeField] private Transform bombContainer;
    [SerializeField] private Color burnColor;
    private Transform cameraParentTransform;

    private ChargeController chargeScript;

    [SerializeField] private float rotSpeed, moveSpeed, bombThrowForce, deviationSpeed, bombReloadTime, downForceWhenBackwardsMagnitude;
    private float deviationTime, deviationRandomForce, deviationExtraForce = 1, rotationDifference, timeUntilNextBomb, Yvelocity, lastCameraYpos, downForceWhenBackwards, lastDownForceWhenFrontflip, leftCameraCornerXpos, targetCameraXpos;

    static public bool dead;
    private bool isPaused, willShotWhenPossible, canDropBombWithClick = true;
    public bool SetIsPaused
    {
        set { isPaused = value; }
    }
    public bool SetCanDropBomb
    {
        set { canDropBombWithClick = value; }
    }

    private Rigidbody2D rb;

    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();

        chargeScript = GameObject.Find("Canvas/Charge").GetComponent<ChargeController>();
        chargeScript.SetDefaultTimeUntilNextBomb = bombReloadTime;
        
        transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 5, 0)).x, 2, transform.position.z);

        cameraParentTransform = Camera.main.transform.parent;

        lastCameraYpos = cameraParentTransform.position.y;
        dead = false;

        leftCameraCornerXpos = Camera.main.ScreenToWorldPoint(new Vector2(-1, 0)).x;
        targetCameraXpos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 5, 0)).x;
    }

    void Update()
    {
        if (!dead)RotateAndMove();

        if (timeUntilNextBomb > 0) timeUntilNextBomb -= Time.deltaTime;
        else
        {
            timeUntilNextBomb = 0;
            if (willShotWhenPossible && !dead && !isPaused) DropBomb();
            willShotWhenPossible = false;
        }

        if (Input.GetButtonDown("Jump") && !dead && !isPaused) DropBomb();
        if (Input.GetButtonDown("Fire1") && !dead && !isPaused && canDropBombWithClick) DropBomb();

        if ((transform.position.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y || transform.position.y > Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y + 0.5f) && !isPaused)
        {
            GenerateExplosion();

            // Die just then for avoiding multiple explosions in the same place.
            Die();
        }
        if (transform.position.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y - 10)
        {
            GameObject.Find("________________Canvas________________").GetComponent<HudController>().DeadPanelOut();
            Destroy(gameObject);
        }

        // Appear from left

        MapGenerator.playerDistanceToStandardPos += (1 - MapGenerator.playerDistanceToStandardPos) * Time.deltaTime;
        transform.position = new Vector3(Mathf.Lerp(leftCameraCornerXpos, targetCameraXpos, MapGenerator.playerDistanceToStandardPos), transform.position.y, transform.position.z);
    }

    void RotateAndMove()
    {
        if (!isPaused)
        {
            float movement = (int)Input.GetAxisRaw("Horizontal");

            float multiplierInCaseOfFrontFlip = 1;
            if (movement > 0 && transform.eulerAngles.z < 150)
            {
                multiplierInCaseOfFrontFlip = Mathf.Cos((transform.eulerAngles.z + 75) / 47.75f) * 0.4f + 1;
                // Basically, the more straight-down you are facing, the more slower you'll be able to rotate forward.
                // I divide the angles by 47.75 instead of 57.3 (180 / pi, explained why in a comment below) so that the amplitude of the wave is 300 units,
                // what divided into two is 150, instead of 360 units. I do that because I want that the curve starts in 0 and finishes in 150.
                
                transform.position += new Vector3(0, (multiplierInCaseOfFrontFlip - 1) * 4 * Time.deltaTime, 0);
                //pushing the ship down so that it's difficult to do a frontflip.
            }

            rb.AddTorque(-movement * multiplierInCaseOfFrontFlip * rotSpeed * Time.deltaTime);

            Deviation(Input.GetButton("Horizontal"));
        }

        float lastYpos = transform.position.y;
        float forceToAddFormula = Mathf.Cos(-transform.eulerAngles.z / 57.3f - Mathf.PI / 2);

        LoopDownForce();

        Vector3 forceToAdd = new Vector3(0, forceToAddFormula * moveSpeed * ((ObjectPassingBy.speedMultiplier - 1) / 2.5f + 1) * Time.deltaTime, 0);
        transform.position += forceToAdd;

        UpdateYposInFunctionOfCameraPos();

        Yvelocity = (transform.position.y - lastYpos) / Time.deltaTime;
    }

    void LoopDownForce()
    {
        float additionDownForceWhenBackwards = Mathf.Cos((transform.eulerAngles.z - 180) / 57.3f) * 0.8f - 0.2f;
        // I divide the rotation between 57.3 (180 / pi) so that the amplitude of the wave is 360 units, instead of 2pi units, to representate it in degrees.

        if (additionDownForceWhenBackwards > 0) additionDownForceWhenBackwards *= 3;
        downForceWhenBackwards += additionDownForceWhenBackwards * downForceWhenBackwardsMagnitude * Time.deltaTime;
        if (downForceWhenBackwards > 0) downForceWhenBackwards = 0;

        transform.position += new Vector3(0, downForceWhenBackwards * Time.deltaTime, 0);
    }

    void UpdateYposInFunctionOfCameraPos()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - (cameraParentTransform.position.y - lastCameraYpos), transform.position.z);

        lastCameraYpos = cameraParentTransform.position.y;
    }

    void DropBomb()
    {
        if (timeUntilNextBomb != 0)
        {
            if (timeUntilNextBomb < 0.25f) willShotWhenPossible = true;
            return;
        }

        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Vector2 bombStartVelocity = direction.normalized * bombThrowForce + new Vector2(0, Yvelocity);

        float speedAdder = (Mathf.Cos(transform.eulerAngles.z / 57.3f) + 0.6f) * 0.625f;

        bombStartVelocity -= new Vector2(speedAdder * 7f * ObjectPassingBy.realSpeedMultiplier, 0);

        Instantiate(bombPrefab, transform.position, Quaternion.identity, bombContainer).GetComponent<Rigidbody2D>().velocity = bombStartVelocity * (ObjectPassingBy.realSpeedMultiplier / 1.5f);
        
        timeUntilNextBomb = bombReloadTime / ObjectPassingBy.realSpeedMultiplier;

        chargeScript.DropBomb(timeUntilNextBomb);
    }

    void Deviation(bool isMoving)
    {
        if (deviationTime <= 0) ChangeDeviation();
        deviationTime -= Time.unscaledDeltaTime;

        if (Input.GetButtonDown("Horizontal")) rotationDifference = transform.eulerAngles.z;
        if (Input.GetButtonUp("Horizontal"))
        {
            rotationDifference = Mathf.Abs((transform.eulerAngles.z - rotationDifference) / 100);
            deviationExtraForce = 15 * rotationDifference;
            if (deviationExtraForce > 8) deviationExtraForce = 8;
        }
        if (deviationExtraForce > 1) deviationExtraForce -= Time.deltaTime * 7;
        else deviationExtraForce = 1;

        if (isMoving) rb.AddTorque(deviationRandomForce * deviationSpeed * deviationExtraForce * 0.3f * Time.deltaTime);
        else rb.AddTorque(deviationRandomForce * deviationSpeed * deviationExtraForce * Time.deltaTime);
    }

    void ChangeDeviation()
    {
        if (deviationExtraForce == 1)
        {
            deviationTime = Random.Range(0.1f, 0.3f);
            deviationRandomForce = Random.Range(-1f, 1f);
        }
        else
        {
            deviationTime = 0.18f;

            if (deviationRandomForce < 0) deviationRandomForce = 0.5f;
            else deviationRandomForce = -0.5f;
        }
    }

    void GenerateExplosion()
    {
        if (dead) return;

        Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        Cursor.visible = true;
        transform.GetComponent<AudioListener>().enabled = false;
        GameObject.Find("Camera/CameraRiser/Main Camera/VignetteEffect").GetComponent<VignetteEffectController>().Explosion(true);

        for (int childNum = 0; childNum < transform.Find("Parts").childCount; childNum++) transform.Find("Parts").GetChild(childNum).GetComponent<SpriteRenderer>().color = burnColor;

        RandomDeathImpulse();
    }

    void RandomDeathImpulse()
    {
        rb.gravityScale = 2;
        rb.angularDrag = 0;

        rb.AddForce(new Vector2(Random.Range(-3f, 5f), Random.Range(10f, 15f)), ForceMode2D.Impulse);

        float randomTorque = Random.Range(120f, 360f);
        if (Random.value > 0.5f) randomTorque *= -1;

        rb.AddTorque(randomTorque);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Explosion") Die();

        // If you throw a bomb to something and just then you crash with it two explosions are generated in a very short distance.
        // That may look a little bit weird, so I have to find a way to nullify one of them. ->

        if (other.gameObject.layer == 3)
        {
            GenerateExplosion();
        }
    }
}
