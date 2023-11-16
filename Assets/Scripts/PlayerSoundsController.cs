using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundsController : MonoBehaviour
{
    private List<AudioSource> audioSources = new List<AudioSource>();

    private float timeSpeed;
    public float SetTimeSpeed
    {
        set { timeSpeed = value; }
    }

    void Start()
    {
        audioSources.Add(transform.Find("WindSound").GetComponent<AudioSource>());
    }

    void Update()
    {
        SetVolumes();
    }

    void SetVolumes()
    {
        if (PlayerController.dead)
        {
            for (int idx = 0; idx < audioSources.Count; idx++) audioSources[idx].volume -= Time.deltaTime;
        }
        else
        {
            for (int idx = 0; idx < audioSources.Count; idx++) audioSources[idx].volume = timeSpeed;
        }
    }
}
