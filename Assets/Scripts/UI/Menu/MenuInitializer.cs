using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInitializer : MonoBehaviour
{
    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("MainVolumeValue");
        Time.timeScale = 1;
    }
}
