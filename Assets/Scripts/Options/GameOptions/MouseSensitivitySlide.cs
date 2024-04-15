using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivitySlide : MonoBehaviour
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

    void Update()
    {
        AdjustColor();
    }

    private void AdjustColor()
    {
        float adjustedSliderValue = (mySlider.value - 0.5f) / 2.5f;
        // The real slider goes from 0.2 to 2.5. This formula converts this value to the relative one between 0 and 1.

        handleImage.color = Color.Lerp(colorLeft, colorRight, adjustedSliderValue);
    }

    private void InitializeValue()
    {
        if (PlayerPrefs.HasKey("MouseSensitivityValue"))
        {
            mySlider.value = PlayerPrefs.GetFloat("MouseSensitivityValue");
        }
        else
        {
            mySlider.value = 1f;
            PlayerPrefs.SetFloat("MouseSensitivityValue", mySlider.value);
        }
    }

    public void ValueChanged()
    {
        PlayerPrefs.SetFloat("MouseSensitivityValue", mySlider.value);
    }
}
