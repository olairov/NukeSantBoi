using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadGatePart : MonoBehaviour
{
    private float safeDistanceFromCamera;

    [SerializeField] private bool isRight;

    void Start()
    {
        safeDistanceFromCamera = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x * 2.5f;
    }

    public void Move(float positionLerp, float yDifferenceFromCamera) // When a scene has just been loaded.
    {
        if (isRight) transform.position = new Vector3(Mathf.Lerp(0, safeDistanceFromCamera, positionLerp), 
            Mathf.Lerp(0, safeDistanceFromCamera * yDifferenceFromCamera, positionLerp), 
            transform.position.z);

        else transform.position = new Vector3(Mathf.Lerp(0, -safeDistanceFromCamera, positionLerp),
            Mathf.Lerp(0, -safeDistanceFromCamera * yDifferenceFromCamera, positionLerp),
            transform.position.z);
    }

    public void PosZero()
    {
        transform.position = new Vector3(0, 0, transform.position.z);
    }
}
