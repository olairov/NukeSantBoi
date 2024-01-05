using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPassingBy : MonoBehaviour
{
    private Transform cameraTransform, playerTransform;

    private Vector3 lastFramePos;

    public static float speedMultiplier, realSpeedMultiplier;
    public float passingSpeed, realPassingSpeed;
    private float appearingDistance = 10, lastCameraYpos, speedAdder;

    [SerializeField] private bool background, fakePassingSpeed;
    public bool appearingObject;

    private void Start()
    {
        if (background) appearingDistance = 30;

        if (!appearingObject) transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + appearingDistance, transform.position.y, transform.position.z);

        cameraTransform = Camera.main.transform.parent;
        if (fakePassingSpeed) playerTransform = GameObject.Find("Player").transform;

        lastFramePos = transform.position;
    }

    private void Update()
    {
        UpdateXpos();
        UpdateYpos();
    }

    private void FixedUpdate()
    {
        // Make that objects also move with their own script are unaffected by the change of speedMultiplier when in loops;
        MovementFix();
    }

    void UpdateXpos()
    {
        transform.position += new Vector3(-passingSpeed, 0, 0) * Time.deltaTime * speedMultiplier * MapGenerator.playerDistanceToStandardPos;
        transform.position += new Vector3(-realPassingSpeed, 0, 0) * Time.deltaTime * realSpeedMultiplier * MapGenerator.playerDistanceToStandardPos;

        if (transform.position.x < Camera.main.ScreenToWorldPoint(Vector3.zero).x - appearingDistance) Destroy(gameObject);
    }

    void MovementFix()
    {
        if (!fakePassingSpeed || playerTransform == null) return;

        if (!PlayerController.dead) speedAdder = (Mathf.Cos(playerTransform.eulerAngles.z / 57.3f) + 0.6f) * 0.625f;
        if (speedAdder < 0) speedAdder = 0;

        transform.position += new Vector3(speedAdder * 7f * Time.deltaTime * realSpeedMultiplier, 0, 0);
    }

    void UpdateYpos()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - (cameraTransform.position.y - lastCameraYpos) * passingSpeed, transform.position.z);

        lastCameraYpos = cameraTransform.position.y;
    }
}
