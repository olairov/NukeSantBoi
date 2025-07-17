using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHandler : MonoBehaviour
{
    [SerializeField] float rotationAmplitude, additionalRotation, woobleSpeed, woobleForce, rotateForeverSpeed;
    float rotateForeverSpeedRandomizer, lastFrameParentRotation;

    [SerializeField] Material burntMaterial;

    [SerializeField] bool imPlayerPlane, XAxis, YAxis, ZAxis;
    bool dead;

    [SerializeField] GameObject fireParticles;

    private void Start()
    {
        
    }

    void Update()
    {
        if (!dead)
        {
            if (imPlayerPlane) PlayerPlaneRotationHandling();
            else EnemyPlaneRotationHandling();
        }
        else RotateForever();
    }

    void PlayerPlaneRotationHandling()
    {
        if (PlayerController.dead)
        {
            Die();
            return;
        }

        RotateModelXAxis(CorrectInputs.verticalAxis * rotationAmplitude);
    }

    void EnemyPlaneRotationHandling()
    {
        if (transform.parent.GetComponent<EnemyMissileController>().dead)
        {
            Die();
            return;
        }

        float rotationFactor = lastFrameParentRotation - transform.parent.localEulerAngles.z;
        RotateModelXAxis(rotationFactor * rotationAmplitude);

        lastFrameParentRotation = transform.parent.localEulerAngles.z;
    }

    void RotateModelXAxis(float rotationIndexInDegrees)
    {
        float woobleDisplacement = Mathf.Sin(Time.time * woobleSpeed) * woobleForce * (imPlayerPlane ? 1 : Time.deltaTime);
        float rotation = rotationIndexInDegrees + additionalRotation + woobleDisplacement;

        transform.localEulerAngles = new Vector3(XAxis ? rotation : transform.localEulerAngles.x, YAxis ? rotation : transform.localEulerAngles.y, ZAxis ? rotation : transform.localEulerAngles.z);
    }


    // Death Related Functions

    void ChangeMaterial(Material newMaterial)
    {
        GetComponent<MeshRenderer>().material = newMaterial;
    }

    void RotateForever()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * rotateForeverSpeed * rotateForeverSpeedRandomizer);
    }

    void Die()
    {
        rotateForeverSpeedRandomizer = Random.value;
        if (Random.value >= 0.5) rotateForeverSpeedRandomizer *= -1;

        if (fireParticles != null) fireParticles.SetActive(true);
        dead = true;

        ChangeMaterial(burntMaterial);
    }
}
