using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPassingBy : MonoBehaviour
{
    public static float speedMultiplyer = 1;
    [SerializeField] private float passingSpeed;
    private float appearingDistance = 5;

    [SerializeField] private bool appearingObject, background;

    private void Update()
    {
        transform.position += new Vector3(-passingSpeed, 0, 0) * Time.deltaTime * speedMultiplyer;

        if (transform.position.x < Camera.main.ScreenToWorldPoint(Vector3.zero).x - appearingDistance) Destroy(gameObject);
    }

    private void Start()
    {
        if (background) appearingDistance = 30;

        if (!appearingObject) transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + appearingDistance, transform.position.y, transform.position.z);
    }
}
