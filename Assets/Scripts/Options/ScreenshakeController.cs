using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshakeController : MonoBehaviour
{
    private ShakeController screenShakeScript;
    private Slider mySlider;

    private RawImage handleImage;
    [SerializeField] private Color colorLeft, colorRight;

    void Start()
    {
        mySlider = transform.GetComponent<Slider>();
        screenShakeScript = Camera.main.GetComponent<ShakeController>();
        handleImage = transform.Find("Handle Slide Area/Handle/HandleBase").GetComponent<RawImage>();

        InitializeValue();
    }

    private void Update()
    {
        handleImage.color = Color.Lerp(colorLeft, colorRight, mySlider.value);
    }

    private void InitializeValue()
    {
        if (PlayerPrefs.HasKey("ScreenshakeValue"))
        {
            mySlider.value = PlayerPrefs.GetFloat("ScreenshakeValue");
        }
        else
        {
            mySlider.value = 0.5f;
            PlayerPrefs.SetFloat("ScreenshakeValue", mySlider.value);
        }
    }

    public void ValueChanged()
    {
        screenShakeScript.SetDefinitiveMaxRadius(mySlider.value);
        PlayerPrefs.SetFloat("ScreenshakeValue", mySlider.value);
    }
}
