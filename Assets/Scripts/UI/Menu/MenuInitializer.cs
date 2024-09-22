using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInitializer : MonoBehaviour
{
    [SerializeField] bool eraseALLStats;

    private void Awake()
    {
        if (eraseALLStats) PlayerPrefs.DeleteAll();
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("MainVolumeValue")) AudioListener.volume = PlayerPrefs.GetFloat("MainVolumeValue");
        else AudioListener.volume = 0.5f;
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.touchCount > 0) TouchControllersManager.isUsingPhone = true;
    }
}
