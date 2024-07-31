using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsInitializer : MonoBehaviour
{
    [SerializeField] private bool imFromGameScene;

    private void Start()
    {
        StartSceneParameters();
    }

    public void StartSceneParameters()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("MainVolumeValue");
        Camera.main.GetComponent<ShakeController>().SetDefinitiveMaxRadius(PlayerPrefs.GetFloat("ScreenshakeValue"));
    }
}
