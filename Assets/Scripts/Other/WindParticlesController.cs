using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParticlesController : MonoBehaviour
{
    private Transform windLineParticlesTransform;
    private ParticleSystem windLineParticles;

    private float cameraX0inUnits;

    void Start()
    {
        windLineParticlesTransform = transform.Find("WindParticles");
        windLineParticles = windLineParticlesTransform.GetComponent<ParticleSystem>();

        windLineParticlesTransform.position = new Vector3(Camera.main.ScreenToWorldPoint(Vector3.one * Screen.width).x + 3, 0, -30);

        cameraX0inUnits = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 4;
    }

    void Update()
    {
        DeleteRemainingParticles();
    }

    void DeleteRemainingParticles()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[windLineParticles.particleCount];
        windLineParticles.GetParticles(particles);

        for (int particle = 0; particle < windLineParticles.particleCount; particle++)
        {
            if (particles[particle].position.x < cameraX0inUnits)
            {
                particles[particle].remainingLifetime = 0.0f;
                Debug.Log(particles[particle].position);
            }
            Debug.Log(particles.Length);
        }
    }
}
