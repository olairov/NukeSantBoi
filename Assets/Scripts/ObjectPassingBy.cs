using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPassingBy : MonoBehaviour
{
    private Transform cameraTransform;

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
    }

    private void Update()
    {
        UpdateXpos();
        UpdateYpos();
    }

    void UpdateXpos()
    {
        if (fakePassingSpeed) transform.position += new Vector3(-passingSpeed, 0, 0) * Time.deltaTime * speedMultiplier * MapGenerator.playerDistanceToStandardPos;
        transform.position += new Vector3(-passingSpeed, 0, 0) * Time.deltaTime * speedMultiplier * MapGenerator.playerDistanceToStandardPos;
        transform.position += new Vector3(-realPassingSpeed, 0, 0) * Time.deltaTime;

        if (transform.position.x < Camera.main.ScreenToWorldPoint(Vector3.zero).x - appearingDistance) Destroy(gameObject);
    }

    void UpdateYpos()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - (cameraTransform.position.y - lastCameraYpos) * passingSpeed, transform.position.z);

        lastCameraYpos = cameraTransform.position.y;
    }
}
