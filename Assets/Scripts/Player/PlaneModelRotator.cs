using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneModelRotator : MonoBehaviour
{
    [SerializeField] float rotationAmplitudeInDegrees, additionalRotation, woobleSpeed, woobleForce, rotateForeverSpeed;
    float rotateForeverSpeedRandomizer;

    [SerializeField] Material burntMaterial;

    [SerializeField] bool imPlayerPlane;
    bool dead;

    GameObject playerFireParticles;

    private void Start()
    {
        if (imPlayerPlane)
        {
            playerFireParticles = GameObject.Find("PlayerFireParticles");
            playerFireParticles.SetActive(false);
        }
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
            ChangeMaterial(burntMaterial);
            Die();

            return;
        }

        RotateModelXAxis(CorrectInputs.verticalAxis * rotationAmplitudeInDegrees);
    }

    void EnemyPlaneRotationHandling()
    {

    }

    void RotateModelXAxis(float rotationIndexInDegrees)
    {
        float woobleDisplacement = Mathf.Sin(Time.time * woobleSpeed) * woobleForce;
        transform.localEulerAngles = new Vector3(rotationIndexInDegrees + additionalRotation + woobleDisplacement, transform.localEulerAngles.y, transform.localEulerAngles.z);
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

        if (imPlayerPlane) playerFireParticles.SetActive(true);
        dead = true;
    }
}
