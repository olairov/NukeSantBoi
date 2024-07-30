using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeOptionsScript : MonoBehaviour
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
    }

    private void InitializeValue()
    {
        if (PlayerPrefs.HasKey("MusicVolumeValue"))
        {
            mySlider.value = PlayerPrefs.GetFloat("MusicVolumeValue");
        }
        else
        {
            mySlider.value = 0.7f;
            PlayerPrefs.SetFloat("MusicVolumeValue", mySlider.value);
        }

        MusicSound.volumeMultiplier = mySlider.value;
    }

    public void ValueChanged()
    {
        float valueToSave = Mathf.Round(mySlider.value * 100f) / 100f;

        MusicSound.volumeMultiplier = valueToSave;
        PlayerPrefs.SetFloat("MusicVolumeValue", valueToSave);
    }
}
