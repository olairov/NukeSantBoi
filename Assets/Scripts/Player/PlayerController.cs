using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] ObjectPool bombDropAnimPool, bombPool, explosionPool;
    [SerializeField] private Color burnColor;
    private Transform cameraRiserTransform, targetTransform;

    private Vector2 savedShootingDirection;

    private ChargeController chargeScript;

    private Animator losingSpeedAnimator;

    private AudioSource bombThrowSound;

    private SpriteRenderer arrowSprite;
    [SerializeField] private Color unchargedArrowColor;
    private Color chargedArrowColor;

    protected BaseMovement movement;

    [SerializeField] private float bombReloadTime, regularBuildingPassingSpeed, appearInSceneSpeed, bombReloadSpeedIncrementMultiplier, bombStartingDistance;
    private float timeUntilNextBomb, Yvelocity, lastCameraYpos, leftCameraCornerXpos, targetCameraXpos;
    public float GetPlayerYvelocity
    {
        get { return Yvelocity; }
    }

    static public bool dead;
    private bool isPaused, willShotWhenPossible, canDropBombWithClick = true;
    public bool SetCanDropBomb
    {
        set { canDropBombWithClick = value; }
    }

    private Rigidbody2D rb;

    void Start()
    {
        dead = false;

        rb = transform.GetComponent<Rigidbody2D>();

        if (PlayerPrefs.HasKey("Level")) SetMovementType(PlayerPrefs.GetInt("Level"));
        else SetMovementType(1);
        movement.SetPlayerStats();

        chargeScript = GameObject.Find("Canvas/Charge").GetComponent<ChargeController>();
        chargeScript.SetDefaultTimeUntilNextBomb = bombReloadTime;
        
        transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 5, 0)).x, 2, transform.position.z);

        cameraRiserTransform = Camera.main.transform.parent;
        targetTransform = GameObject.Find("Target").transform;

        losingSpeedAnimator = GameObject.Find("Canvas/Warning/LosingSpeedWarning").GetComponent<Animator>();
        bombThrowSound = transform.Find("Sounds/BombThrow").GetComponent<AudioSource>();
        arrowSprite = transform.Find("TouchArrow").GetComponent<SpriteRenderer>();
        chargedArrowColor = arrowSprite.color;

        lastCameraYpos = cameraRiserTransform.position.y;

        leftCameraCornerXpos = Camera.main.ScreenToWorldPoint(new Vector2(-1, 0)).x;
        targetCameraXpos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 5, 0)).x;
    }

    public void SetMovementType(int level)
    {
        if (dead) return;

        switch (level)
        {
            case 2:
                movement = gameObject.AddComponent<MovementTypeB>();
                break;
            case 3:
                movement = gameObject.AddComponent<MovementTypeC>();
                break;
            case 4:
                movement = gameObject.AddComponent<MovementTypeD>();
                break;
            default:
                movement = gameObject.AddComponent<MovementTypeA>();
                break;
        }
        movement.Init(transform, rb);

        GameObject.Find("________________Canvas________________").GetComponent<HudController>().GiveActualLevel = level;
    }

    void Update()
    {
        // Enable this and set BombReloadTime to 0 if you're bored.
        //DropBomb(targetTransform.position - transform.position);

        float lastYpos = transform.position.y;
        if (!dead)
        {
            movement.MovementProcess();
            UpdateYposInFunctionOfCameraPos();
        }
        Yvelocity = (transform.position.y - lastYpos) / Time.deltaTime;

        if (timeUntilNextBomb > 0) timeUntilNextBomb -= Time.deltaTime;
        else
        {
            arrowSprite.color = chargedArrowColor;
            if (willShotWhenPossible && !dead && !isPaused) DropBomb(savedShootingDirection);
            willShotWhenPossible = false;
        }

        if (!TouchControllersManager.isUsingPhone)
        {
            if (Input.GetButtonDown("Jump") && !dead && !isPaused) DropBomb(targetTransform.position - transform.position);
            if (Input.GetButtonDown("Fire1") && !dead && !isPaused && canDropBombWithClick) DropBomb(targetTransform.position - transform.position);
        }

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

        MapGenerator.playerDistanceToStandardPos += (1 - MapGenerator.playerDistanceToStandardPos) * Time.deltaTime * appearInSceneSpeed;
        transform.position = new Vector3(Mathf.Lerp(leftCameraCornerXpos, targetCameraXpos, MapGenerator.playerDistanceToStandardPos), transform.position.y, transform.position.z);
    }

    void UpdateYposInFunctionOfCameraPos()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - (cameraRiserTransform.localPosition.y - lastCameraYpos) * (regularBuildingPassingSpeed + 1), transform.position.z);
        // Multiplied by regularBuildingPassingSpeed to also take into account the scenary's movement in function of the camera movement.

        lastCameraYpos = cameraRiserTransform.localPosition.y;
    }

    public void DropBomb(Vector2 direction)
    {
        direction.Normalize();
        if (timeUntilNextBomb > 0)
        {
            if (timeUntilNextBomb < 0.2f) // If the player shoots a little before the bomb is charged, It will shoot it when it's available.
            {
                willShotWhenPossible = true;
                savedShootingDirection = direction;
            }
            return;
        }

        PlayDropBombSound();

        float myRotationIndicator = Mathf.Cos((transform.eulerAngles.z - 180) / 57.3f) + 0.25f; // This is so that the bomb doesn't try going forward when launched while looping.
        if (myRotationIndicator > 1) myRotationIndicator = 1;

        GameObject bomb = bombPool.GetObject(true);
        bomb.transform.position = transform.position + new Vector3(direction.x, direction.y, 0) * bombStartingDistance;
        bomb.GetComponent<BombController>().SetDirection(direction, new Vector2(0, Yvelocity), myRotationIndicator);

        timeUntilNextBomb = bombReloadTime / (ObjectPassingBy.realSpeedMultiplier * bombReloadSpeedIncrementMultiplier);

        chargeScript.DropBomb(timeUntilNextBomb);

        arrowSprite.color = unchargedArrowColor;

        SimpleBombDropAnimation(direction);
    }

    void SimpleBombDropAnimation(Vector2 bombDirection)
    {
        GameObject newBombDropAnim = bombDropAnimPool.GetObject(true);

        newBombDropAnim.transform.position = transform.position + new Vector3(bombDirection.x, bombDirection.y, 0);
        newBombDropAnim.transform.up = bombDirection;
        newBombDropAnim.GetComponent<Rigidbody2D>().velocity = new Vector2(0, Yvelocity);
    }

    void PlayDropBombSound()
    {
        bombThrowSound.Stop();

        bombThrowSound.pitch = Random.Range(0.9f, 1.3f);

        bombThrowSound.Play();
    }

    void GenerateExplosion()
    {
        if (dead) return;

        explosionPool.GetObject(true).transform.position = transform.position;
    }

    public void LosingSpeedWarning(bool enabled)
    {
        losingSpeedAnimator.SetBool("Enabled", enabled);
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        LosingSpeedWarning(false);

        transform.Find("TouchArrow").gameObject.SetActive(false);
        Cursor.visible = true;
        GameObject.Find("Canvas/VignetteEffect").GetComponent<VignetteEffectController>().Explosion(true);

        for (int childNum = 0; childNum < transform.Find("Parts").childCount; childNum++) transform.Find("Parts").GetChild(childNum).GetComponent<SpriteRenderer>().color = burnColor;

        RandomDeathImpulse();
    }

    void RandomDeathImpulse()
    {
        rb.gravityScale = 2;
        rb.angularDrag = 0;
        rb.angularVelocity = 0;

        rb.AddForce(new Vector2(Random.Range(-3f, 5f), Random.Range(10f, 15f)), ForceMode2D.Impulse);

        float randomTorque = Random.Range(120f, 360f);
        if (Random.value > 0.5f) randomTorque *= -1;

        rb.AddTorque(randomTorque);
    }

    public void SetIsPaused(bool paused)
    {
        isPaused = paused;
        movement.IsPaused = paused;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Explosion") && Mathf.Abs(Vector2.Distance(other.transform.position, transform.position)) < 2f) Die();

        // If you throw a bomb to something and just then you crash with it two explosions are generated in a very short distance.
        // That may look a little bit weird, so I have to find a way to nullify one of them. ->

        if (other.gameObject.layer == 3 || other.gameObject.layer == 6)
        {
            GenerateExplosion();
        }
    }
}
