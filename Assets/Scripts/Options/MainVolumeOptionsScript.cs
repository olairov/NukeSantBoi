using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainVolumeOptionsScript : MonoBehaviour
{
    private Slider mySlider;

    void Start()
    {
        mySlider = transform.GetComponent<Slider>();

        InitializeValue();
    }
    
    private void InitializeValue()
    {
        if (PlayerPrefs.HasKey("MainVolumeValue"))
        {
            mySlider.value = PlayerPrefs.GetFloat("MainVolumeValue");
        }
        else
        {
            mySlider.value = 0.5f;
            PlayerPrefs.SetFloat("MainVolumeValue", mySlider.value);
        }

        AudioListener.volume = mySlider.value;
    }

    public void ValueChanged()
    {
        AudioListener.volume = mySlider.value;
        PlayerPrefs.SetFloat("MainVolumeValue", mySlider.value);
    }
}
