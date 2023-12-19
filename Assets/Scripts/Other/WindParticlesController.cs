using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParticlesController : MonoBehaviour
{
    private Transform windLineParticlesTransform;
    private ParticleSystem windLineParticles;

    void Start()
    {
        windLineParticlesTransform = transform.Find("WindParticles");
        windLineParticles = windLineParticlesTransform.GetComponent<ParticleSystem>();

        windLineParticlesTransform.position = new Vector3(Camera.main.ScreenToWorldPoint(Vector3.one * Screen.width).x + 3, 0, -30);
    }

    void Update()
    {
        for (int particle = 0; particle < windLineParticles.particleCount; particle++)
        {
            ParticleSystem.GetParticles(windLineParticles)
        }
    }
}
