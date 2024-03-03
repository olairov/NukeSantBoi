using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsInitializer : MonoBehaviour
{
    private ShakeController cameraShakeScript;

    private void Start()
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
