using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainVolumeOptionsScript : MonoBehaviour
{
    private Slider mySlider;

    private RawImage handleImage;
    [SerializeField] private Color colorLeft, colorRight;

    void Start()
    {
        mySlider = transform.GetComponent<Slider>();
        handleImage = transform.Find("Handle Slide Area/Handle/HandleBase").GetComponent<RawImage>();
        
        InitializeValue();
    }

    private void Update()
    {
        handleImage.color = Color.Lerp(colorLeft, colorRight, mySlider.value);
        mySlider.value = PlayerPrefs.GetFloat("MainVolumeValue");
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
        float valueToSave = Mathf.Round(mySlider.value * 100f) / 100f;
        
        AudioListener.volume = valueToSave;
        PlayerPrefs.SetFloat("MainVolumeValue", valueToSave);
    }
}
