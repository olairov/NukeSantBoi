using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceVolumeOptionsScript : MonoBehaviour
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
        if (PlayerPrefs.HasKey("InterfaceVolumeValue"))
        {
            mySlider.value = PlayerPrefs.GetFloat("InterfaceVolumeValue");
        }
        else
        {
            mySlider.value = 1f;
            PlayerPrefs.SetFloat("InterfaceVolumeValue", mySlider.value);
        }

        UISound.volumeMultiplier = mySlider.value;
    }

    public void ValueChanged()
    {
        float valueToSave = Mathf.Round(mySlider.value * 100f) / 100f;

        UISound.volumeMultiplier = valueToSave;
        PlayerPrefs.SetFloat("InterfaceVolumeValue", valueToSave);
    }
}
