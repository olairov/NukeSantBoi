using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButton : MonoBehaviour
{
    private ShakeController cameraShakeScript;
    private OptionsSlideController optionsSlideScript;

    void Start()
    {
        optionsSlideScript = GameObject.Find("CanvasOptions/Options").GetComponent<OptionsSlideController>();

        if (GameObject.Find("Camera/CameraRiser/Main Camera")) cameraShakeScript = GameObject.Find("Camera/CameraRiser/Main Camera").GetComponent<ShakeController>();
        else if (GameObject.Find("Camera/Main Camera")) cameraShakeScript = GameObject.Find("Camera/Main Camera").GetComponent<ShakeController>();
        else cameraShakeScript = GameObject.Find("OptionsCamera/Main Camera").GetComponent<ShakeController>();
    }

    public void BackPressed()
    {
        optionsSlideScript.OptionsExit();
        GameObject.Find("OptionsInitializer").GetComponent<OptionsInitializer>().StartEverything();
    }

    public void ShakePressed()
    {
        if (PlayerPrefs.HasKey("ScreenshakeValue")) cameraShakeScript.SetDefinitiveMaxRadius(PlayerPrefs.GetFloat("ScreenshakeValue"));
        else cameraShakeScript.SetDefinitiveMaxRadius(0.5f);

        cameraShakeScript.Shake();
    }

    public void MoreOptionsButton()
    {
        optionsSlideScript.GoToAdvancedOptions();
    }

    public void LessOptionsButton()
    {
        optionsSlideScript.ComeFromAdvancedOptions();
    }
}
