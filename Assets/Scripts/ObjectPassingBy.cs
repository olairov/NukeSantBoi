using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPassingBy : MonoBehaviour
{
    private Transform cameraTransform, playerTransform;

    private Vector3 lastFramePos;

    public static float speedMultiplier, realSpeedMultiplier;
    public float passingSpeed, realPassingSpeed;
    private float appearingDistance = 10, lastCameraYpos;

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

    void UpdateXpos()
    {
        Vector3 realVelocity = transform.position - lastFramePos;
        
        transform.position += new Vector3(-passingSpeed, 0, 0) * Time.deltaTime * speedMultiplier * MapGenerator.playerDistanceToStandardPos;
        transform.position += new Vector3(-realPassingSpeed, 0, 0) * Time.deltaTime;

        if (transform.position.x < Camera.main.ScreenToWorldPoint(Vector3.zero).x - appearingDistance) Destroy(gameObject);

        // Make that objects also move with their own script are unaffected by the change of speedMultiplier when in loops;

        if (!fakePassingSpeed || playerTransform == null) return;

        float speedMultiplierFactor = Mathf.Cos(playerTransform.eulerAngles.z / 57.3f) * 1f + 0.6f;
        Debug.Log(speedMultiplierFactor);
        if (speedMultiplierFactor < 0) speedMultiplierFactor = 0;

        transform.position += new Vector3(speedMultiplierFactor * realSpeedMultiplier * Time.deltaTime, 0, 0);
    }

    void UpdateYpos()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - (cameraTransform.position.y - lastCameraYpos) * passingSpeed, transform.position.z);

        lastCameraYpos = cameraTransform.position.y;
    }
}
