using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPartParallaxer : MonoBehaviour
{
    [SerializeField] float startXrot, endXrot;
    [SerializeField] float startYpos, endYpos;
    [SerializeField] float startYsize, endYsize;

    [SerializeField] bool controlledByRotationForce;

    private EnemyPlaneController myEnemyPlaneController;

    void Start()
    {
        if (controlledByRotationForce) myEnemyPlaneController = transform.parent.parent.GetComponent<EnemyPlaneController>();
    }

    void Update()
    {
        if (controlledByRotationForce) ChangeRotation();
        else if (!PlayerController.dead) ChangeRotation();
    }

    void ChangeRotation()
    {
        if (Time.timeScale < 1) return;

        float lerpValue = controlledByRotationForce? myEnemyPlaneController.actualRotationSpeed / 100 + 0.5f : CorrectInputs.verticalAxis / 2 + 0.5f;
        lerpValue = Mathf.Clamp01(lerpValue);

        transform.localEulerAngles = new Vector3(Mathf.Lerp(startXrot, endXrot, lerpValue), 0, 180);
        if (startYpos != 0 && endYpos != 0) transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(startYpos, endYpos, lerpValue), transform.localPosition.z);
        transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(startYsize, endYsize, lerpValue), transform.localScale.z);
    }
}
