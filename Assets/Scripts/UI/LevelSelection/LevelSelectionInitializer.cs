using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionInitializer : MonoBehaviour
{
    void Start()
    {
        InitializeStats();
    }

    void InitializeStats()
    {
        if (SceneManager.sceneCount > 1)
        {
            GameObject.Find("LevelSelectionCamera").SetActive(false);
            GetComponent<Canvas>().worldCamera = GameObject.Find("Camera/Main Camera").GetComponent<Camera>();

            GameObject.Find("EventSystemLevelSelection").SetActive(false);
        }
        else
        {
            GetComponent<Canvas>().worldCamera = GameObject.Find("LevelSelectionCamera/Main Camera").GetComponent<Camera>();

            AudioListener.volume = PlayerPrefs.GetFloat("MainVolumeValue");
            Camera.main.GetComponent<ShakeController>().SetDefinitiveMaxRadius(PlayerPrefs.GetFloat("ScreenshakeValue"));
        }
    }

    private void Update()
    {
        if (Input.touchCount > 0) TouchControllersManager.isUsingPhone = true;
    }
}
