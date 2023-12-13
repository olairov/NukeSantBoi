using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform playerTransform;

    private float cameraWidthInUnits, playerOutProgress;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

        cameraWidthInUnits = GetComponent<Camera>().ScreenToWorldPoint(new Vector2(0, Screen.width)).x - GetComponent<Camera>().ScreenToWorldPoint(Vector2.zero).x;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (PlayerController.dead) return;

        transform.position = new Vector3(playerTransform.position.x + cameraWidthInUnits / Mathf.Lerp(2, 4, playerOutProgress), transform.position.y, transform.position.z);

        // Make player appear from left

        playerOutProgress += Time.deltaTime * (1 - playerOutProgress);
    }
}
