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

    [SerializeField] private float rotSpeed, moveSpeed, bombThrowForce, deviationSpeed, bombReloadTime;
    private float deviationTime, deviationRandomForce, deviationExtraForce = 1, rotationDifference, timeUntilNextBomb, Yvelocity, lastCameraYpos;

    static public bool dead;
    private bool isPaused, willShotWhenPossible;
    public bool SetIsPaused
    {
        set { isPaused = value; }
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
    }

    void Update()
    {
        if (!dead)RotateAndMove();

        if ((Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")) && !dead && !isPaused) DropBomb();
        if (timeUntilNextBomb > 0) timeUntilNextBomb -= Time.deltaTime;
        else
        {
            timeUntilNextBomb = 0;
            if (willShotWhenPossible && !dead && !isPaused) DropBomb();
            willShotWhenPossible = false;
        }

        if ((transform.position.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y || transform.position.y > Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y) && !isPaused)
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

        MapGenerator.playerDistanceToObjective += (1 - MapGenerator.playerDistanceToObjective) * Time.deltaTime;
        transform.position = new Vector3(Mathf.Lerp(Camera.main.ScreenToWorldPoint(new Vector2(-1, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 5, 0)).x, MapGenerator.playerDistanceToObjective), transform.position.y, transform.position.z);
    }

    void RotateAndMove()
    {
        if (!isPaused)
        {
            int movement = (int)Input.GetAxisRaw("Horizontal");
            if (Input.GetAxisRaw("Vertical") != 0) movement = -(int)Input.GetAxisRaw("Vertical");

            if ((movement < 0 && transform.eulerAngles.z < 230) || (movement > 0 && transform.eulerAngles.z > 130)) rb.AddTorque(-movement * rotSpeed * Time.deltaTime);

            Deviation(Input.GetButton("Horizontal"));
        }

        float lastYpos = transform.position.y;
        Vector3 forceToAdd = new Vector3(0, (transform.eulerAngles.z - 180) / 90 * moveSpeed * ((ObjectPassingBy.speedMultiplyer - 1) / 2.5f + 1) * Time.deltaTime, 0);
        transform.position += forceToAdd;

        UpdateYposInFunctionOfCameraPos();

        Yvelocity = (transform.position.y - lastYpos) / Time.deltaTime;
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
            if (timeUntilNextBomb < 0.2f) willShotWhenPossible = true;
            return;
        }

        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Vector2 bombStartVelocity = direction.normalized * bombThrowForce + new Vector2(0, Yvelocity);

        GameObject newBomb = Instantiate(bombPrefab, transform.position, Quaternion.identity, bombContainer);
        newBomb.GetComponent<Rigidbody2D>().velocity = bombStartVelocity * ObjectPassingBy.speedMultiplyer / 1.5f;

        timeUntilNextBomb = bombReloadTime / ObjectPassingBy.speedMultiplyer;

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
            deviationTime = 0.2f;

            if (deviationRandomForce < 0) deviationRandomForce = 1;
            else deviationRandomForce = -1;
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

        transform.GetComponent<SpriteRenderer>().color = burnColor;

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
