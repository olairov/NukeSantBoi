using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainVolumeOptionsScript : MonoBehaviour
{
    private Slider mySlider;

    private RawImage handleImage;
    [SerializeField] private Color colorLeft, colorRight;
    [SerializeField] private Slider otherMainVolumeSlider;

    static private bool changedByCode;

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
        changedByCode = true;
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

        if (!changedByCode)
        {
            changedByCode = true;
            otherMainVolumeSlider.value = valueToSave;
        }
        else changedByCode = false;
        //otherMainVolumeSlider.value = valueToSave;

        AudioListener.volume = valueToSave;
        PlayerPrefs.SetFloat("MainVolumeValue", valueToSave);
    }
}
