using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsInitializer : MonoBehaviour
{
    [SerializeField] private float defaultVolume = 0.3f, defaultGameVolume = 1, defaultUiVolume = 1, DefaultMusicVolume = 0.6f;

    private void Start()
    {
        StartSceneParameters();
    }

    public void StartSceneParameters()
    {
        if (PlayerPrefs.HasKey("MainVolumeValue"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("MainVolumeValue");
        }
        else
        {
            AudioListener.volume = defaultVolume;
            PlayerPrefs.SetFloat("MainVolumeValue", defaultVolume);
        }

        // Init Game Volume
        if (PlayerPrefs.HasKey("GameVolumeValue"))
        {
            InGameSound.volumeMultiplier = PlayerPrefs.GetFloat("GameVolumeValue");
        }
        else
        {
            InGameSound.volumeMultiplier = defaultGameVolume;
            PlayerPrefs.SetFloat("GameVolumeValue", defaultGameVolume);
        }

        // Init UI Volume
        if (PlayerPrefs.HasKey("InterfaceVolumeValue"))
        {
            UISound.volumeMultiplier = PlayerPrefs.GetFloat("InterfaceVolumeValue");
        }
        else
        {
            UISound.volumeMultiplier = defaultUiVolume;
            PlayerPrefs.SetFloat("InterfaceVolumeValue", defaultUiVolume);
        }

        // Init Music Volume
        if (PlayerPrefs.HasKey("MusicVolumeValue"))
        {
            MusicSound.volumeMultiplier = PlayerPrefs.GetFloat("MusicVolumeValue");
        }
        else
        {
            MusicSound.volumeMultiplier = DefaultMusicVolume;
            PlayerPrefs.SetFloat("MusicVolumeValue", DefaultMusicVolume);
        }

        // 3D Sound
        if (PlayerPrefs.HasKey("ThreeDAudio"))
        {
            InGameSound.threeDsound = PlayerPrefs.GetInt("ThreeDAudio") > 0;
        }
        else
        {
            InGameSound.threeDsound = true;
            PlayerPrefs.SetInt("ThreeDAudio", 1);
        }

        Camera.main.GetComponent<ShakeController>().SetDefinitiveMaxRadius(PlayerPrefs.GetFloat("ScreenshakeValue"));
    }
}
