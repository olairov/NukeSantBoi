using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    [SerializeField] float pitchMin, pitchMax, volumeMin, volumeMax;

    [SerializeField] private List<AudioClip> availableSounds = new();
    
    public void PlayAudio()
    {
        AudioSource myAudioSource = GetComponent<AudioSource>();

        myAudioSource.pitch = Random.Range(pitchMin, pitchMax);
        myAudioSource.volume = Random.Range(volumeMin, volumeMax);

        float randomValue = Random.value;
        if (availableSounds.Count <= 0)
        {
            myAudioSource.Play();
            return;
        }
        float eachSoundProbability = 1 / availableSounds.Count;

        // Randomly select one audio from the list and assign it:
        for (int availableSoundsNum = availableSounds.Count - 1; availableSoundsNum >= 0; availableSoundsNum--)
        {
            if (randomValue >= eachSoundProbability * availableSoundsNum)
            {
                myAudioSource.clip = availableSounds[availableSoundsNum];
                break;
            }
        }

        myAudioSource.Play();
    }
}
