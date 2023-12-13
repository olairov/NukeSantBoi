using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSoundController : MonoBehaviour
{
    [SerializeField] private AudioClip Explosion2, Explosion3;
    
    // Start is called before the first frame update
    void Start()
    {
        ChoseAudio();
    }

    void ChoseAudio()
    {
        AudioSource myAudio = transform.GetComponent<AudioSource>();
        float randomValue = Random.value;

        if (randomValue > 0.66f) myAudio.clip = Explosion2;
        else if (randomValue > 0.33f) myAudio.clip = Explosion3;

        myAudio.pitch = Random.Range(0.8f, 1.3f);
        myAudio.Play();
    }
}
