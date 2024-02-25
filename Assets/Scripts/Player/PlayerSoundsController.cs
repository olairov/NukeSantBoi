using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundsController : MonoBehaviour
{
    private List<AudioSource> audioSources = new List<AudioSource>();
    private List<float> audioVolumes = new List<float>();

    private float timeSpeed, boostAudioVolume;

    [SerializeField] private bool hasBoost;
    private AudioSource boostAudio;

    public float SetTimeSpeed
    {
        set { timeSpeed = value; }
    }

    void Start()
    {
        for (int childNum = 0; childNum < transform.childCount; childNum++) audioSources.Add(transform.GetChild(childNum).GetComponent<AudioSource>());
        for (int childNum = 0; childNum < transform.childCount; childNum++) audioVolumes.Add(transform.GetChild(childNum).GetComponent<AudioSource>().volume);

        if (hasBoost)
        {
            boostAudio = transform.parent.Find("Boost").GetComponent<AudioSource>();
            boostAudioVolume = boostAudio.volume;
        }
    }

    void Update()
    {
        SetVolumes();
    }

    void SetVolumes()
    {
        if (PlayerController.dead)
        {
            for (int idx = 0; idx < audioSources.Count; idx++) audioSources[idx].volume -= audioVolumes[idx] * Time.deltaTime;
        }
        else
        {
            for (int idx = 0; idx < audioSources.Count; idx++) audioSources[idx].volume = audioVolumes[idx] * timeSpeed;
            if (hasBoost) boostAudio.volume = boostAudioVolume * timeSpeed;
        }
    }
}
