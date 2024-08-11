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

    private void Update()
    {
        if (Input.touchCount > 0) TouchControllersManager.isUsingPhone = true;
    }
}
