using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsInitializer : MonoBehaviour
{
    private ShakeController cameraShakeScript;

    void Start()
    {
        cameraShakeScript = Camera.main.GetComponent<ShakeController>();

        StartEverything();
    }

    public void StartEverything()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("MainVolumeValue");
        cameraShakeScript.SetDefinitiveMaxRadius(PlayerPrefs.GetFloat("ScreenshakeValue"));
    }
}
