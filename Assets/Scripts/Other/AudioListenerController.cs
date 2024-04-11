using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerController : MonoBehaviour
{
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

        transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 5, 0)).x, 0, playerTransform.position.z);
    }

    void Update()
    {
        if (!PlayerController.dead) transform.position = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z);
    }
}
