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
        cameraShakeScript = GameObject.Find("Camera/CameraRiser/Main Camera").GetComponent<ShakeController>();
    }

    public void BackPressed()
    {
        optionsSlideScript.OptionsExit();
        GameObject.Find("OptionsInitializer").GetComponent<OptionsInitializer>().StartEverything();
    }

    public void ShakePressed()
    {
        cameraShakeScript.SetDefinitiveMaxRadius(PlayerPrefs.GetFloat("ScreenshakeValue"));
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
